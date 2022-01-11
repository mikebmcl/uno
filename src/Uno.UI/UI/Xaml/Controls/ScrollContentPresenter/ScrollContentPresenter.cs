using Uno.Extensions;
using Uno.Foundation.Logging;
using Uno.UI.DataBinding;
using Windows.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Uno.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Foundation;
#if XAMARIN_ANDROID
using View = Android.Views.View;
using Font = Android.Graphics.Typeface;
#elif XAMARIN_IOS_UNIFIED
using UIKit;
using View = UIKit.UIView;
using Color = UIKit.UIColor;
using Font = UIKit.UIFont;
#elif __MACOS__
using AppKit;
using View = AppKit.NSView;
using Color = AppKit.NSColor;
using Font = AppKit.NSFont;
#else
using View = Windows.UI.Xaml.UIElement;
#endif

namespace Windows.UI.Xaml.Controls
{
	public partial class ScrollContentPresenter : ContentPresenter, ILayoutConstraints
	{
		public ScrollContentPresenter()
		{
			InitializePartial();
			RegisterAsScrollPort(this);
		}
		partial void InitializePartial();

		#region ScrollOwner
		private ManagedWeakReference _scroller;

		public object ScrollOwner
		{
			get => _scroller.Target;
			set
			{
				if (_scroller is { } oldScroller)
				{
					WeakReferencePool.ReturnWeakReference(this, oldScroller);
				}

				_scroller = WeakReferencePool.RentWeakReference(this, value);
			}
		}
		#endregion

		private ScrollViewer Scroller => ScrollOwner as ScrollViewer;

#if __WASM__
		bool _forceChangeToCurrentView;

		bool IScrollContentPresenter.ForceChangeToCurrentView
		{
			get => _forceChangeToCurrentView;
			set => _forceChangeToCurrentView = value;
		}
#endif

		private void InitializeScrollContentPresenter()
		{
			this.RegisterParentChangedCallback(this, OnParentChanged);
		}

		private void OnParentChanged(object instance, object key, DependencyObjectParentChangedEventArgs args)
		{
			// Set Content to null when we are removed from TemplatedParent. Otherwise Content.RemoveFromSuperview() may
			// be called when it is attached to a different view.
			if (args.NewParent == null)
			{
				Content = null;
			}
		}

		bool ILayoutConstraints.IsWidthConstrained(View requester)
		{
			if (requester != null && CanHorizontallyScroll)
			{
				return false;
			}

			return this.IsWidthConstrainedSimple() ?? (Parent as ILayoutConstraints)?.IsWidthConstrained(this) ?? false;
		}

		bool ILayoutConstraints.IsHeightConstrained(View requester)
		{
			if (requester != null && CanVerticallyScroll)
			{
				return false;
			}

			return this.IsHeightConstrainedSimple() ?? (Parent as ILayoutConstraints)?.IsHeightConstrained(this) ?? false;
		}

		public double ExtentHeight
		{
			get
			{
				if (Content is FrameworkElement fe)
				{
					var explicitHeight = fe.Height;
					if (!explicitHeight.IsNaN())
					{
						return explicitHeight;
					}
					var canUseActualHeightAsExtent =
						ActualHeight > 0 &&
						fe.VerticalAlignment == VerticalAlignment.Stretch;
					var extentHeightAdjustment = fe.Margin.Top + fe.Margin.Bottom;
					return canUseActualHeightAsExtent ? fe.ActualHeight + extentHeightAdjustment : fe.DesiredSize.Height + extentHeightAdjustment;
				}

				return 0d;
			}
		}

		public double ExtentWidth
		{
			get
			{
				if (Content is FrameworkElement fe)
				{
					var explicitWidth = fe.Width;
					if (!explicitWidth.IsNaN())
					{
						return explicitWidth;
					}

					var canUseActualWidthAsExtent =
						ActualWidth > 0 &&
						fe.HorizontalAlignment == HorizontalAlignment.Stretch;
					var extentWidthAdjustment = fe.Margin.Left + fe.Margin.Right;
					return canUseActualWidthAsExtent ? fe.ActualWidth + extentWidthAdjustment : fe.DesiredSize.Width + extentWidthAdjustment;
				}

				return 0d;
			}
		}

		public double ViewportHeight => DesiredSize.Height - (Margin.Top + Margin.Bottom);

		public double ViewportWidth => DesiredSize.Width - (Margin.Left + Margin.Right);

#if UNO_HAS_MANAGED_SCROLL_PRESENTER || __WASM__
		protected override Size MeasureOverride(Size size)
		{
			if (Content is UIElement child)
			{
				var slotSize = size;

#if __WASM__
				if (CanVerticallyScroll || _forceChangeToCurrentView)
				{
					slotSize.Height = double.PositiveInfinity;
				}
				if (!CanVerticallyScroll)
				{
					if (child is FrameworkElement fe)
					{
						// The slot size we got is reduced by the margin... of this. It should be reduced by the margin of the child if it can't scroll.
						slotSize.Height = Math.Max(0, slotSize.Height - (fe.Margin.Top + fe.Margin.Bottom));
					}
				}
				if (CanHorizontallyScroll || _forceChangeToCurrentView)
				{
					slotSize.Width = double.PositiveInfinity;
				}
				if (!CanHorizontallyScroll)
				{
					if (child is FrameworkElement fe)
					{
						// The slot size we got is reduced by the margin... of this. It should be reduced by the margin of the child if it can't scroll.
						slotSize.Width = Math.Max(0, slotSize.Width - (fe.Margin.Left + fe.Margin.Right));
					}
				}
#else
				if (CanVerticallyScroll)
				{
					slotSize.Height = double.PositiveInfinity;
				}
				else
				{
					if (child is FrameworkElement fe)
					{
						// The slot size we got is reduced by the margin... of this. It should be reduced by the margin of the child if it can't scroll.
						slotSize.Height = Math.Max(0, slotSize.Height - (fe.Margin.Top + fe.Margin.Bottom));
					}
				}
				if (CanHorizontallyScroll)
				{
					slotSize.Width = double.PositiveInfinity;
				}
				else
				{
					if (child is FrameworkElement fe)
					{
						// The slot size we got is reduced by the margin... of this. It should be reduced by the margin of the child if it can't scroll.
						slotSize.Width = Math.Max(0, slotSize.Width - (fe.Margin.Left + fe.Margin.Right));
					}
				}
#endif
				child.Measure(slotSize);

				var desired = child.DesiredSize;

				// Give opportunity to the the content to define the viewport size itself
				(child as ICustomScrollInfo)?.ApplyViewport(ref desired);

				return new Size(
					Math.Min(size.Width, desired.Width),
					Math.Min(size.Height, desired.Height)
				);
			}

			return new Size(0, 0);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (Content is UIElement child)
			{
				Rect childRect = default;

				var desiredSize = child.DesiredSize;

				childRect.Width = Math.Max(finalSize.Width, desiredSize.Width);
				childRect.Height = Math.Max(finalSize.Height, desiredSize.Height);

				child.Arrange(childRect);

#if __WASM__
				// NOTE: This code path is unnecessary if the method ApplyScrollContentPresenterContent ScrollViewer.cs does not contain the workaround of
				//	wrapping content that has a non-default Margin value in a Border control with no styling (or other FrameworkElement if Border was unsuitable).
				if (child is FrameworkElement fe && fe.Margin != default)
				{
					// We rely on the Tag to let us know that we have the workaround in place
					if (fe.Tag is string tag && tag == ScrollViewer.UnoIssue7000WASMorkaroundTagValue)
					{
						if (fe is Border b)
						{
							// Give opportunity to the the content to define the viewport size itself; the Border's Child is the actual user content
							// and the Border should not occupy any space so the custom viewport, if any, should work as the user intended.
							(b.Child as ICustomScrollInfo)?.ApplyViewport(ref finalSize);
						}
						else
						{
							// The type of control used as a wrapper was changed in ScrollViewer.ApplyScrollContentPresenterContent without updating this code.
							_logDebug?.Debug($"Expected {nameof(child)} to be of type {nameof(Border)}; instead it is of type {fe.GetType().FullName}.");
							// Give opportunity to the the content to define the viewport size itself; this is unlikely to work unless the wrapper was setup to
							// implement ICustomScrollInfo and forward the call to ApplyViewport to the actual content.
							(child as ICustomScrollInfo)?.ApplyViewport(ref finalSize);
						}
					}
					else
					{
						// The Tag check failed so presumably the workaround was removed. Assume that child is the actual user Content and proceed.
						// Give opportunity to the the content to define the viewport size itself
						_logDebug?.Debug($"{nameof(child)} has a Margin but the check for a wrapper using Tag == {nameof(ScrollViewer)}.{nameof(ScrollViewer.UnoIssue7000WASMorkaroundTagValue)} failed. Assuming the workaround was removed and there is no wrapper.");
						(child as ICustomScrollInfo)?.ApplyViewport(ref finalSize);
					}
				}
				else
				{
					// Give opportunity to the the content to define the viewport size itself
					(child as ICustomScrollInfo)?.ApplyViewport(ref finalSize);
				}
#else
				// Give opportunity to the the content to define the viewport size itself
				(child as ICustomScrollInfo)?.ApplyViewport(ref finalSize);
#endif
			}

			return finalSize;
		}

		internal override bool IsViewHit()
			=> true;
#elif __IOS__ // Note: No __ANDROID__, the ICustomScrollInfo support is made directly in the NativeScrollContentPresenter
		protected override Size MeasureOverride(Size size)
		{
			var result = base.MeasureOverride(size);

			(RealContent as ICustomScrollInfo).ApplyViewport(ref result);

			return result;
		}

		/// <inheritdoc />
		protected override Size ArrangeOverride(Size finalSize)
		{
			var result = base.ArrangeOverride(finalSize);

			(RealContent as ICustomScrollInfo).ApplyViewport(ref result);

			return result;
		}
#endif
	}
}
