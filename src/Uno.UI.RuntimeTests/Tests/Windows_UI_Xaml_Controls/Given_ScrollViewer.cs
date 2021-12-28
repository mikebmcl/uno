using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Private.Infrastructure;
using static Private.Infrastructure.TestServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml_Controls
{
	[TestClass]
	[RunsOnUIThread]
	public class Given_ScrollViewer
	{
		private ResourceDictionary _testsResources;

		public Style ScrollViewerCrowdedTemplateStyle => _testsResources["ScrollViewerCrowdedTemplateStyle"] as Style;

		[TestInitialize]
		public void Init()
		{
			_testsResources = new TestsResources();
		}

#if __SKIA__ || __WASM__
		[TestMethod]
		public async Task When_CreateVerticalScroller_Then_DoNotLoadAllTemplate()
		{
			var sut = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollMode = ScrollMode.Enabled,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				HorizontalScrollMode = ScrollMode.Disabled,
				Height = 100,
				Width = 100,
				Content = new Border { Height = 200, Width = 50 }
			};
			WindowHelper.WindowContent = sut;

			await WindowHelper.WaitForIdle();

			var buttons = sut
				.EnumerateAllChildren(maxDepth: 256)
				.OfType<RepeatButton>()
				.Count();

			Assert.IsTrue(buttons > 0); // We make sure that we really loaded the right template
			Assert.IsTrue(buttons <= 4);
		}


		[TestMethod]
		public async Task When_NonScrollableScroller_Then_DoNotLoadAllTemplate()
		{
			var sut = new ScrollViewer
			{
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				VerticalScrollMode = ScrollMode.Enabled,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
				HorizontalScrollMode = ScrollMode.Disabled,
				Height = 100,
				Width = 100,
				Content = new Border { Height = 50, Width = 50 }
			};
			WindowHelper.WindowContent = sut;

			await WindowHelper.WaitForIdle();

			var buttons = sut
				.EnumerateAllChildren(maxDepth: 256)
				.OfType<RepeatButton>()
				.Count();

			Assert.IsTrue(buttons == 0);
		}

		[TestMethod]
		public async Task When_HorizontallyScrollableTextBox_Then_DoNotLoadAllScrollerTemplate()
		{
			var sut = new TextBox
			{
				Width = 100,
				Text = "Hello world, this a long text that would cause the TextBox to enable horizontal scroll, so we should find some RepeatButton in the children of this TextBox."
			};
			WindowHelper.WindowContent = sut;

			await WindowHelper.WaitForIdle();

			var bars = sut
				.EnumerateAllChildren(maxDepth: 256)
				.OfType<ScrollBar>()
				.Count();

			Assert.IsTrue(bars == 0); // TextBox is actually not using scrollbars!
		}

		[TestMethod]
		public async Task When_NonScrollableTextBox_Then_DoNotLoadAllScrollerTemplate()
		{
			var sut = new TextBox
			{
				Width = 100,
				Text = "42"
			};
			WindowHelper.WindowContent = sut;

			await WindowHelper.WaitForIdle();

			var bars = sut
				.EnumerateAllChildren(maxDepth: 256)
				.OfType<ScrollBar>()
				.Count();

			Assert.IsTrue(bars == 0); // TextBox is actually not using scrollbars!
		}
#endif

		[TestMethod]
		[RunsOnUIThread]
		public async Task When_ScrollViewer_Resized()
		{
			var content = new Border
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Background = new SolidColorBrush(Colors.Cyan)
			};

			var sut = new ScrollViewer { Content = content };

			var container = new Border { Child = sut };

			WindowHelper.WindowContent = container;

			await WindowHelper.WaitForLoaded(content);

			using var _ = new AssertionScope();

			await CheckForSize(200, 400, "Initial");
			await CheckForSize(250, 450, "Growing 1st time");
			await CheckForSize(225, 425, "Shringking 1st time");
			await CheckForSize(16, 16, "Super-shrinking");
			await CheckForSize(200, 400, "Back Original");

			async Task CheckForSize(int width, int height, string name)
			{
				container.Width = width;
				container.Height = height;

				await WindowHelper.WaitForLoaded(content);

				await WindowHelper.WaitForIdle();

				using var _ = new AssertionScope($"{name} [{width}x{height}]");

#if !NETFX_CORE
				sut.ViewportMeasureSize.Width.Should().Be(width, "ViewportMeasureSize.Width");
				sut.ViewportMeasureSize.Height.Should().Be(height, "ViewportMeasureSize.Height");

				sut.ViewportArrangeSize.Width.Should().Be(width, "ViewportArrangeSize.Width");
				sut.ViewportArrangeSize.Height.Should().Be(height, "ViewportArrangeSize.Height");
#endif

				sut.ExtentWidth.Should().Be(width, "Extent");
				sut.ExtentHeight.Should().Be(height, "Extent");

				sut.ActualWidth.Should().Be(width, "ScrollViewer ActualWidth");
				sut.ActualHeight.Should().Be(height, "ScrollViewer ActualHeight");
				sut.RenderSize.Width.Should().Be(width, "ScrollViewer RenderSize.Width");
				sut.RenderSize.Height.Should().Be(height, "ScrollViewer RenderSize.Height");

				content.ActualWidth.Should().Be(width, "content ActualWidth");
				content.ActualHeight.Should().Be(height, "content ActualHeight");
				content.RenderSize.Width.Should().Be(width, "content RenderSize.Width");
				content.RenderSize.Height.Should().Be(height, "content RenderSize.Height");
			}
		}

		[TestMethod]
		public async Task When_Presenter_Doesnt_Take_Up_All_Space()
		{
			const int ContentWidth = 700;
			var content = new Ellipse
			{
				Width = ContentWidth,
				VerticalAlignment = VerticalAlignment.Stretch,
				Fill = new SolidColorBrush(Colors.Tomato)
			};
			const double ScrollViewerWidth = 300;
			var SUT = new ScrollViewer
			{
				Style = ScrollViewerCrowdedTemplateStyle,
				Width = ScrollViewerWidth,
				Height = 200,
				Content = content
			};

			WindowHelper.WindowContent = SUT;

			await WindowHelper.WaitForLoaded(content);

			const double ButtonWidth = 29;
			const double PresenterActualWidth = ScrollViewerWidth - 2 * ButtonWidth;
			await WindowHelper.WaitForEqual(ScrollViewerWidth, () => SUT.ActualWidth);
			Assert.AreEqual(PresenterActualWidth, SUT.ViewportWidth);
			Assert.AreEqual(ContentWidth, SUT.ExtentWidth);
			Assert.AreEqual(ContentWidth - PresenterActualWidth, SUT.ScrollableWidth);
			;
		}

		[TestMethod]
		[RunsOnUIThread]
		public async Task When_ScrollViewer_PaddingContentMargin()
		{
			// 250 is arbitrary except in so far as that it must be larger than any combination of the horizontal or vertical combinations of the content margin and scrollviewer padding values below plus the maximum valid difference.
			var width = 250.0;
			var height = 250.0;

			using var _ = new AssertionScope();

			// Allow up to a 1 pixel difference to account for minor differences due to floating point-based rendering calculations on various platforms.
			const double maximumValidDifference = 1;

			// The values are slightly arbitrary but are chosen because they are significantly larger than the maximum valid difference and will not combine into values that make it difficult in the event of an error to discern the exact source value(s) while also ending up with a combined L+R of both and T+B of both that is substantively smaller than the width/height we're using. 
			var contentMargin = new Thickness(19, 23, 26, 32);
			var svPadding = new Thickness(4, 6, 11, 13);

			// The first test is to ensure that the calculated values for given, givenContent, sut, and sutContent in VerifyExpectedDifferences are equal because the remaining three tests would otherwise be invalid.
			await VerifyExpectedDifferences("No Padding and No Content Margin", default, default);
			await VerifyExpectedDifferences($"Has Content Margin [{contentMargin.Left},{contentMargin.Top},{contentMargin.Right},{contentMargin.Bottom}]", default, contentMargin);
			await VerifyExpectedDifferences($"Has ScrollViewer Padding [{svPadding.Left},{svPadding.Top},{svPadding.Right},{svPadding.Bottom}]", svPadding, default);
			await VerifyExpectedDifferences($"Has Both ScrollViewer Padding [{svPadding.Left},{svPadding.Top},{svPadding.Right},{svPadding.Bottom}] and Content Margin [{contentMargin.Left},{contentMargin.Top},{contentMargin.Right},{contentMargin.Bottom}]", svPadding, contentMargin);

			async Task VerifyExpectedDifferences(string name, Thickness svPadding, Thickness contentMargin)
			{
				using var _ = new AssertionScope($"{name}");
				FrameworkElement GenerateContent(Thickness margin, double width, double height)
				{
					var result = new StackPanel
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Stretch,
						Background = new SolidColorBrush(Colors.Cyan),
						MinWidth = width,
						MinHeight = height,
						Margin = margin,
					};
					// Add content sufficient in size to ensure that scrolling will be necessary including allowances for content margin and scrollviewer padding plus the maximum valid difference.
					result.Children.Add(new Rectangle() { Width = width * 2, Height = 1, Fill = new SolidColorBrush(Colors.Red) });
					result.Children.Add(new Rectangle() { Width = 1, Height = height * 2, Fill = new SolidColorBrush(Colors.Red) });
					return result;
				}

				// given and givenContent have no content margin and no padding. They combine to serve as our given to test all of the when-then scenarios, which use sut and sutContent
				var givenContent = GenerateContent(default, width * 2, height * 2);
				var sutContent = GenerateContent(contentMargin, width * 2, height * 2);
				var given = new ScrollViewer { Content = givenContent, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
				var sut = new ScrollViewer { Content = sutContent, Padding = svPadding, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
				Grid.SetRow(given, 0);
				Grid.SetRow(sut, 1);
				// Use a Grid with identical row heights for given and sut so that we ensure the values of sut and sutContent can be appropriately tested against given.
				var container = new Grid() { Width = width, Height = height * 2 };
				container.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				container.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

				WindowHelper.WindowContent = container;

				container.Children.Clear();
				container.Children.Add(given);
				container.Children.Add(sut);
				await WindowHelper.WaitForLoaded(container);
				await WindowHelper.WaitForLoaded(given);
				await WindowHelper.WaitForLoaded(sut);
				await WindowHelper.WaitForLoaded(givenContent);
				await WindowHelper.WaitForLoaded(sutContent);
				await WindowHelper.WaitForIdle();

				sut.ActualWidth.Should().BeApproximately(given.ActualWidth, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.ActualWidth)} should be approximately equal to {given.ActualWidth}");
				sut.ActualHeight.Should().BeApproximately(given.ActualHeight, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.ActualHeight)} should be approximately equal to {given.ActualHeight}");
				sut.RenderSize.Width.Should().BeApproximately(given.RenderSize.Width, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.RenderSize)}.{nameof(sut.RenderSize.Width)} should be approximately equal to {given.RenderSize.Width}");
				sut.RenderSize.Height.Should().BeApproximately(given.RenderSize.Height, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.RenderSize)}.{nameof(sut.RenderSize.Height)} should be approximately equal to {given.RenderSize.Height}");

				var expectedWidthDifference = svPadding.Left + svPadding.Right;
				var expectedHeightDifference = svPadding.Top + svPadding.Bottom;
				sut.ViewportWidth.Should().BeApproximately(given.ViewportWidth - expectedWidthDifference, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.ViewportWidth)} should be {given.ViewportWidth} reduced by {nameof(ScrollViewer)}.{nameof(sut.Padding)} Left + Right {expectedWidthDifference}");
				sut.ViewportHeight.Should().BeApproximately(given.ViewportHeight - expectedHeightDifference, maximumValidDifference, $"{nameof(ScrollViewer)} {nameof(sut.ViewportHeight)} should be {given.ViewportHeight} reduced by {nameof(ScrollViewer)}.{nameof(sut.Padding)} Top + Bottom {expectedHeightDifference}");

				expectedWidthDifference = contentMargin.Left + contentMargin.Right;
				expectedHeightDifference = contentMargin.Top + contentMargin.Bottom;
				sut.ExtentWidth.Should().BeApproximately(given.ExtentWidth + expectedWidthDifference, maximumValidDifference, $"{nameof(sut.ExtentWidth)} should be approximately {given.ExtentWidth} increased by Content.Margin Left + Right value {Math.Abs(expectedWidthDifference)}");
				sut.ExtentHeight.Should().BeApproximately(given.ExtentHeight + expectedHeightDifference, maximumValidDifference, $"{nameof(sut.ExtentHeight)} should be approximately {given.ExtentHeight} increased by Content.Margin Top + Bottom value {Math.Abs(expectedHeightDifference)}");

				sutContent.ActualWidth.Should().BeApproximately(givenContent.ActualWidth, maximumValidDifference, $"Content ActualWidth {sutContent.ActualWidth} should be approximately equal to {givenContent.ActualWidth}");
				sutContent.ActualHeight.Should().BeApproximately(givenContent.ActualHeight, maximumValidDifference, $"Content ActualHeight {sutContent.ActualHeight} should be approximately equal to {givenContent.ActualHeight}");

				sutContent.RenderSize.Width.Should().BeApproximately(givenContent.RenderSize.Width, maximumValidDifference, $"Content {nameof(sutContent.RenderSize)}.{nameof(sutContent.RenderSize.Width)} {sutContent.RenderSize.Width} should be approximately equal to {givenContent.RenderSize.Width}");
				sutContent.RenderSize.Height.Should().BeApproximately(givenContent.RenderSize.Height, maximumValidDifference, $"Content {nameof(sutContent.RenderSize)}.{nameof(sutContent.RenderSize.Height)} {sutContent.RenderSize.Height} should be approximately equal to {givenContent.RenderSize.Height}");

				expectedWidthDifference = contentMargin.Left + contentMargin.Right + svPadding.Left + svPadding.Right;
				expectedHeightDifference = contentMargin.Top + contentMargin.Bottom + svPadding.Top + svPadding.Bottom;
				sut.ScrollableWidth.Should().BeApproximately(given.ScrollableWidth + expectedWidthDifference, maximumValidDifference, $"{nameof(sut.ScrollableWidth)} should be approximately {given.ScrollableWidth} increased by Content.Margin Left + Right + ScrollViewer.Padding Left + Right {Math.Abs(expectedWidthDifference)}");
				sut.ScrollableHeight.Should().BeApproximately(given.ScrollableHeight + expectedHeightDifference, maximumValidDifference, $"{nameof(sut.ScrollableHeight)} should be approximately {given.ScrollableHeight} increased by Content.Margin Top + Bottom + ScrollViewer.Padding Top + Bottom {Math.Abs(expectedHeightDifference)}");

				var viewChanged = false;
				void OnViewChanged(object __, ScrollViewerViewChangedEventArgs ___)
				{
					sut.ViewChanged -= OnViewChanged;
					viewChanged = true;
				}

				// WASM bug check: When scrolling all the way to the ScrollableWidth then all the way to the ScrollableHeight (trying to do both in one call will result in false values being reported), the scroll to ScrollableHeight should leave HorizontalOffset approximately equal to ScrollableWidth. Scrolling in the reverse order should leave VerticalOffset approximately equal to ScrollableHeight. 
				sut.ViewChanged += OnViewChanged;
				// Only do a horizontal scroll halfway so that second horizontal scroll call, which scrolls all the way, will result in ViewChanged event firing.
				var scrollTo = sut.ScrollableWidth / 2;
				var firstHorizontalScroll = scrollTo;
				sut.ChangeView(scrollTo, null, null, true);
				await WindowHelper.WaitForIdle();
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.{nameof(sut.ChangeView)}({scrollTo}, null, null, true) call (first horizontal)");
				sut.HorizontalOffset.Should().BeGreaterThan(0, $"{nameof(sut.HorizontalOffset)} {sut.HorizontalOffset} should be greater than 0");

				viewChanged = false;
				sut.ViewChanged += OnViewChanged;
				scrollTo = sut.ScrollableHeight;
				sut.ChangeView(null, scrollTo, null, true);
				await WindowHelper.WaitForIdle();
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.{nameof(sut.ChangeView)}(null, {scrollTo}, null, true) call (vertical)");
				sut.VerticalOffset.Should().BeGreaterThan(0, $"{nameof(sut.VerticalOffset)} value {sut.VerticalOffset} should be greater than 0");

				viewChanged = false;
				sut.ViewChanged += OnViewChanged;
				scrollTo = sut.ScrollableWidth;
				sut.ChangeView(scrollTo, null, null, true);
				await WindowHelper.WaitForIdle();
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.{nameof(sut.ChangeView)}({scrollTo}, null, null, true) call (second horizontal)");
				var horizontalOffsetCheck = Math.Min(firstHorizontalScroll + 1, sut.ScrollableWidth);
				sut.HorizontalOffset.Should().BeGreaterOrEqualTo(horizontalOffsetCheck, $"{nameof(sut.HorizontalOffset)} value {sut.HorizontalOffset} should be greater than or equal to {horizontalOffsetCheck}");

				sut.HorizontalOffset.Should().BeApproximately(sut.ScrollableWidth, maximumValidDifference, $"{nameof(sut.HorizontalOffset)} when scrolled all the way should be approximately the {nameof(sut.ScrollableWidth)} value {sut.ScrollableWidth}");
				sut.VerticalOffset.Should().BeApproximately(sut.ScrollableHeight, maximumValidDifference, $"{nameof(sut.VerticalOffset)} when scrolled all the way should be approximately the {nameof(sut.ScrollableHeight)} value {sut.ScrollableHeight}");

			}
		}
	}
}
