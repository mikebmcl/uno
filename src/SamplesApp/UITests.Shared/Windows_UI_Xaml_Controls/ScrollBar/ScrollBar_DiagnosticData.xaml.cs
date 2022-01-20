using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

using Uno.UI.Samples.Controls;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UITests.Windows_UI_Xaml_Controls.ScrollBar
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[SampleControlInfo("ScrollBar"
#if __WASM__
		, ignoreInSnapshotTests: true
#endif
		, description: "Provides diagnostic data about the elements and the pointer position."
		)]
	public sealed partial class ScrollBar_DiagnosticData : Page, INotifyPropertyChanged
	{
		public ScrollBar_DiagnosticData()
		{
			this.InitializeComponent();
			PointerEntered += ScrollBar_DiagnosticData_PointerEntered;
			PointerExited += ScrollBar_DiagnosticData_PointerExited;
			RootGrid.PointerEntered += RootGrid_PointerEntered;
			RootGrid.PointerExited += RootGrid_PointerExited;
			TestGrid.PointerEntered += TestGrid_PointerEntered;
			TestGrid.PointerExited += TestGrid_PointerExited;
			ControlsGrid.LayoutUpdated += ControlsGrid_LayoutUpdated;
			ControlsGrid.SizeChanged += ControlsGrid_SizeChanged;
			ControlsGrid.PointerEntered += ControlsGrid_PointerEntered;
			ControlsGrid.PointerExited += ControlsGrid_PointerExited;

			TestBorder.PointerEntered += TestBorder_PointerEntered;
			TestBorder.PointerExited += TestBorder_PointerExited;

			HorizontalScrollBar.LayoutUpdated += HorizontalScrollBar_LayoutUpdated;
			HorizontalScrollBar.SizeChanged += HorizontalScrollBar_SizeChanged;
			HorizontalScrollBar.PointerEntered += HorizontalScrollBar_PointerEntered;
			HorizontalScrollBar.PointerExited += HorizontalScrollBar_PointerExited;
			HorizontalScrollBar.PointerPressed += HorizontalScrollBar_PointerPressed;
			HorizontalScrollBar.PointerReleased += HorizontalScrollBar_PointerReleased;
			HorizontalScrollBar.PointerCaptureLost += HorizontalScrollBar_PointerCaptureLost;
			HorizontalScrollBar.PointerCanceled += HorizontalScrollBar_PointerCanceled;

			VerticalScrollBar.LayoutUpdated += VerticalScrollBar_LayoutUpdated;
			VerticalScrollBar.SizeChanged += VerticalScrollBar_SizeChanged;
			VerticalScrollBar.PointerEntered += VerticalScrollBar_PointerEntered;
			VerticalScrollBar.PointerExited += VerticalScrollBar_PointerExited;
			VerticalScrollBar.PointerPressed += VerticalScrollBar_PointerPressed;
			VerticalScrollBar.PointerReleased += VerticalScrollBar_PointerReleased;
			VerticalScrollBar.PointerCaptureLost += VerticalScrollBar_PointerCaptureLost;
			VerticalScrollBar.PointerCanceled += VerticalScrollBar_PointerCanceled;

			_enableDelayedReleaseDispatcherTimer = new DispatcherTimer();
			_enableDelayedReleaseDispatcherTimer.Tick += _enableDelayedReleaseDispatcherTimer_Tick;
			Unloaded += ScrollBar_DiagnosticData_Unloaded;
		}

		private void ScrollBar_DiagnosticData_Unloaded(object sender, RoutedEventArgs e)
		{
			try
			{
				_enableDelayedReleaseDispatcherTimer.Stop();
			}
			catch (Exception) { }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		private string DebugStringForRect(Rect r) => $"[[X: {r.X:F2} Y: {r.Y:F2}] [R: {r.Right:F2} B: {r.Bottom:F2}] [W: {r.Width:F2} H: {r.Height:F2}]]";
		private string DebugStringForPoint(Point p) => $"[{p.X:F2}, {p.Y:F2}]";

		private const string _noneIndicatorMode = "None";
		private const string _mouseIndicatorMode = "MouseIndicator";
		private const string _touchIndicatorMode = "TouchIndicator";
		public List<string> IndicatorModeComboItemsSource => new List<string>(new string[] { _noneIndicatorMode, _mouseIndicatorMode, _touchIndicatorMode });
		private string _selectedIndicatorMode = _mouseIndicatorMode;
		public string SelectedIndicatorMode
		{
			get => _selectedIndicatorMode;
			set
			{
				if (_selectedIndicatorMode != value)
				{
					_selectedIndicatorMode = value;
					RaisePropertyChanged();
				}
			}
		}

		private DispatcherTimer _enableDelayedReleaseDispatcherTimer;
		private void _enableDelayedReleaseDispatcherTimer_Tick(object sender, object e)
		{
			try
			{
				HorizontalScrollBar.ReleasePointerCaptures();
				VerticalScrollBar.ReleasePointerCaptures();
			}
			catch (Exception)
			{
				_enableDelayedReleaseDispatcherTimer?.Stop();
			}
		}

		private TimeSpan _enableDelayedReleaseDispatcherTimerInterval = new TimeSpan(0, 0, 5);

		private bool? _enableDelayedReleaseIsChecked = false;
		public bool? EnableDelayedReleaseIsChecked
		{
			get => _enableDelayedReleaseIsChecked;
			set
			{
				if (_enableDelayedReleaseIsChecked != value)
				{
					_enableDelayedReleaseIsChecked = value;
					RaisePropertyChanged();
				}
			}
		}

		private int _pageEntered;
		private void ScrollBar_DiagnosticData_PointerEntered(object sender, PointerRoutedEventArgs e) =>
			pagePointerEntered.Text = (++_pageEntered).ToString();

		private void ScrollBar_DiagnosticData_PointerExited(object sender, PointerRoutedEventArgs e) =>
			pagePointerEntered.Text = (--_pageEntered).ToString();

		private int _rootGridEntered;
		private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e) =>
			rootGridPointerEntered.Text = (++_rootGridEntered).ToString();

		private void RootGrid_PointerExited(object sender, PointerRoutedEventArgs e) =>
			rootGridPointerEntered.Text = (--_rootGridEntered).ToString();

		private int _testGridEntered;
		private void TestGrid_PointerEntered(object sender, PointerRoutedEventArgs e) =>
			testGridPointerEntered.Text = (++_testGridEntered).ToString();

		private void TestGrid_PointerExited(object sender, PointerRoutedEventArgs e) =>
			testGridPointerEntered.Text = (--_testGridEntered).ToString();

		private void ControlsGrid_LayoutUpdated(object sender, object args)
		{
			ControlsGrid.LayoutUpdated -= ControlsGrid_LayoutUpdated;
			controlsGridLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(ControlsGrid));
		}

		private void ControlsGrid_SizeChanged(object sender, SizeChangedEventArgs args) =>
			controlsGridLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(ControlsGrid));

		private int _controlsGridEntered;
		private void ControlsGrid_PointerEntered(object sender, PointerRoutedEventArgs args) =>
			controlsGridPointerEntered.Text = (++_controlsGridEntered).ToString();

		private void ControlsGrid_PointerExited(object sender, PointerRoutedEventArgs args) =>
			controlsGridPointerEntered.Text = (--_controlsGridEntered).ToString();

		private int _testBorderEntered;
		private void TestBorder_PointerEntered(object sender, PointerRoutedEventArgs args) =>
			testBorderPointerEntered.Text = (++_testBorderEntered).ToString();

		private void TestBorder_PointerExited(object sender, PointerRoutedEventArgs args) =>
			testBorderPointerEntered.Text = (--_testBorderEntered).ToString();

		private void HorizontalScrollBar_LayoutUpdated(object sender, object args)
		{
			HorizontalScrollBar.LayoutUpdated -= HorizontalScrollBar_LayoutUpdated;
			horizontalScrollBarLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(HorizontalScrollBar));
		}

		private void HorizontalScrollBar_SizeChanged(object sender, SizeChangedEventArgs args) =>
			horizontalScrollBarLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(HorizontalScrollBar));

		private int _horizontalScrollBarEntered;
		private void HorizontalScrollBar_PointerEntered(object sender, PointerRoutedEventArgs args) =>
			horizontalScrollBarPointerEntered.Text = (++_horizontalScrollBarEntered).ToString();

		private void HorizontalScrollBar_PointerExited(object sender, PointerRoutedEventArgs args) =>
			horizontalScrollBarPointerEntered.Text = (--_horizontalScrollBarEntered).ToString();

		private void HorizontalScrollBar_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					(sender as UIElement).CapturePointer(e.Pointer);
					_enableDelayedReleaseDispatcherTimer.Interval = _enableDelayedReleaseDispatcherTimerInterval;
					_enableDelayedReleaseDispatcherTimer.Start();
				}
				catch (Exception) { }
			}
		}

		private void HorizontalScrollBar_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		private void HorizontalScrollBar_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		private void HorizontalScrollBar_PointerCanceled(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		private void VerticalScrollBar_LayoutUpdated(object sender, object args)
		{
			VerticalScrollBar.LayoutUpdated -= VerticalScrollBar_LayoutUpdated;
			verticalScrollBarLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(VerticalScrollBar));
		}

		private void VerticalScrollBar_SizeChanged(object sender, SizeChangedEventArgs args) =>
			verticalScrollBarLayoutSlot.Text = DebugStringForRect(LayoutInformation.GetLayoutSlot(VerticalScrollBar));

		private int _verticalScrollBarEntered;
		private void VerticalScrollBar_PointerEntered(object sender, PointerRoutedEventArgs args) =>
			verticalScrollBarPointerEntered.Text = (++_verticalScrollBarEntered).ToString();

		private void VerticalScrollBar_PointerExited(object sender, PointerRoutedEventArgs args) =>
			verticalScrollBarPointerEntered.Text = (--_verticalScrollBarEntered).ToString();
		private void VerticalScrollBar_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					(sender as UIElement).CapturePointer(e.Pointer);
					_enableDelayedReleaseDispatcherTimer.Interval = _enableDelayedReleaseDispatcherTimerInterval;
					_enableDelayedReleaseDispatcherTimer.Start();
				}
				catch (Exception) { }
			}
		}

		private void VerticalScrollBar_PointerReleased(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		private void VerticalScrollBar_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		private void VerticalScrollBar_PointerCanceled(object sender, PointerRoutedEventArgs e)
		{
			if (EnableDelayedReleaseIsChecked is true)
			{
				try
				{
					_enableDelayedReleaseDispatcherTimer.Stop();
				}
				catch (Exception) { }
			}
		}

		public void OnVerticalScroll(object sender, ScrollEventArgs args) =>
			scrollValue.Text = $"Vertical Scroll: {args.ScrollEventType}, {args.NewValue:F2}";

		public void OnHorizontalScroll(object sender, ScrollEventArgs args) =>
			scrollValue.Text = $"Horizontal Scroll: {args.ScrollEventType}, {args.NewValue:F2}";

		protected override void OnPointerMoved(PointerRoutedEventArgs args)
		{
			base.OnPointerMoved(args);
			var position = args.GetCurrentPoint(null).Position;
			pointerPositionValue.Text = DebugStringForPoint(position);

			position = args.GetCurrentPoint(ControlsGrid).Position;
			pointerPositionRelativeToControlsGridValue.Text = DebugStringForPoint(position);

			position = args.GetCurrentPoint(VerticalScrollBar).Position;
			pointerPositionRelativeToVerticalScrollBarValue.Text = DebugStringForPoint(position);

			position = args.GetCurrentPoint(HorizontalScrollBar).Position;
			pointerPositionRelativeToHorizontalScrollBarValue.Text = DebugStringForPoint(position);

			var originalSourceAsFrameworkElement = args.OriginalSource as FrameworkElement;
			var name = originalSourceAsFrameworkElement?.Name;
			var parentAsFrameworkElement = originalSourceAsFrameworkElement?.Parent as FrameworkElement;
			var parentTypeName = parentAsFrameworkElement?.GetType().Name;
			var parentName = parentAsFrameworkElement?.Name;
			originalSourceType.Text = $"Type: {args.OriginalSource?.GetType().Name ?? "(null)"}";
			originalSourceName.Text = $"Name: {name ?? "(None)"}";
			originalSourceParentType.Text = $"Type: {parentTypeName ?? "(null)"}";
			originalSourceParentName.Text = $"Name: {parentName ?? "(None)"}";
		}
	}
}
