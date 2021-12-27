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

			// 10 and 16 are arbitrary but are chosen because they will not combine into values that make it difficult to discern the source of an error.
			var contentMargin = new Thickness(10);
			var svPadding = new Thickness(16);

			// The first test is to ensure that the calculated values for given, givenContent, sut, and sutContent in VerifyExpectedDifferences are equal because the remaining three tests would otherwise be invalid.
			await VerifyExpectedDifferences("No padding and No Content Margin", default, default);
			await VerifyExpectedDifferences("Has Content Margin", default, contentMargin);
			await VerifyExpectedDifferences("Has Padding", svPadding, default);
			await VerifyExpectedDifferences("Has Both Padding and Content Margin", svPadding, contentMargin);

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

				var viewChanged = false;
				void OnViewChanged(object __, ScrollViewerViewChangedEventArgs ___)
				{
					sut.ViewChanged -= OnViewChanged;
					viewChanged = true;
				}

				sut.ViewChanged += OnViewChanged;
				var scrollTo = sut.ScrollableWidth / 2;
				sut.ChangeView(scrollTo, null, null, true);
				await WindowHelper.WaitForIdle();
				//await WindowHelper.WaitFor(() => sut.HorizontalOffset > 0, 1000, message: $"Waiting for {nameof(sut.HorizontalOffset)} to change from 0.");
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.HorizontalOffset change to {scrollTo}");
				sut.HorizontalOffset.Should().BeGreaterThan(0, $"Checking sut.ChangeView({scrollTo}, null, null, true) call");

				viewChanged = false;
				sut.ViewChanged += OnViewChanged;
				scrollTo = sut.ScrollableHeight;
				sut.ChangeView(null, scrollTo, null, true);
				await WindowHelper.WaitForIdle();
				//await WindowHelper.WaitFor(() => sut.VerticalOffset > 0, 1000, message: $"Waiting for {nameof(sut.VerticalOffset)} to change from 0.");
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.{nameof(sut.VerticalOffset)} change to {scrollTo}");
				sut.VerticalOffset.Should().BeGreaterThan(0, $"Checking sut.ChangeView({scrollTo}, null, null, true) call");

				viewChanged = false;
				sut.ViewChanged += OnViewChanged;
				scrollTo = sut.ScrollableWidth;
				sut.ChangeView(scrollTo, null, null, true);
				await WindowHelper.WaitForIdle();
				var horizontalOffsetCheck = Math.Min(sut.ScrollableWidth / 2 + 1, sut.ScrollableWidth);
				await WindowHelper.WaitFor(() => viewChanged, 1000, message: $"{nameof(sut)}.HorizontalOffset change to {scrollTo}");
				sut.HorizontalOffset.Should().BeGreaterOrEqualTo(horizontalOffsetCheck, $"Checking sut.ChangeView({scrollTo}, null, null, true) call");
				//await WindowHelper.WaitFor(() => sut.HorizontalOffset >= horizontalOffsetCheck, 1000, message: $"Waiting for {nameof(sut.HorizontalOffset)} to change to be greater that or equal to {horizontalOffsetCheck}.");

				sut.ActualWidth.Should().Be(given.ActualWidth, $"ScrollViewer ActualWidth should be exactly equal to {nameof(given)}");
				sut.ActualHeight.Should().Be(given.ActualHeight, $"ScrollViewer ActualHeight should be exactly equal to {nameof(given)}");
				sut.RenderSize.Width.Should().Be(given.RenderSize.Width, $"ScrollViewer RenderSize.Width should be exactly equal to {nameof(given)}");
				sut.RenderSize.Height.Should().Be(given.RenderSize.Height, $"ScrollViewer RenderSize.Height should be exactly equal to {nameof(given)}");

				var expectedWidthDifference = svPadding.Left + svPadding.Right;
				var expectedHeightDifference = svPadding.Top + svPadding.Bottom;
				//#if !NETFX_CORE
				sut.ViewportWidth.Should().BeApproximately(given.ViewportWidth - expectedWidthDifference, maximumValidDifference, $"Viewport.Width should be the {nameof(given)} value {given.ViewportWidth} reduced by ScrollViewer.Padding Left + Right {expectedWidthDifference}");
				sut.ViewportWidth.Should().BeApproximately(given.ViewportHeight - expectedHeightDifference, maximumValidDifference, $"Viewport.Width should be the {nameof(given)} value {given.ViewportHeight} reduced by ScrollViewer.Padding Top + Bottom {expectedHeightDifference}");
				//#endif

				expectedWidthDifference = contentMargin.Left + contentMargin.Right;
				expectedHeightDifference = contentMargin.Top + contentMargin.Bottom;
				sut.ExtentWidth.Should().BeApproximately(given.ExtentWidth + expectedWidthDifference, maximumValidDifference, $"Extent.Width should be the {nameof(given)} value {given.ExtentWidth} increased by Content.Margin Left + Right {Math.Abs(expectedWidthDifference)}");
				sut.ExtentHeight.Should().BeApproximately(given.ExtentHeight + expectedHeightDifference, maximumValidDifference, $"Viewport.Width should be the {nameof(given)} value {given.ExtentHeight} increaded by Content.Margin Top + Bottom {Math.Abs(expectedHeightDifference)}");

				sutContent.ActualWidth.Should().Be(givenContent.ActualWidth, $"Content ActualWidth {sutContent.ActualWidth} should be exactly equal to {nameof(given)} value {given.ActualWidth}");
				sutContent.ActualHeight.Should().Be(givenContent.ActualHeight, $"Content ActualHeight {sutContent.ActualHeight} should be exactly equal to {nameof(given)} value {given.ActualHeight}");

				sutContent.RenderSize.Width.Should().Be(givenContent.RenderSize.Width, $"Content RenderSize.Width {sutContent.RenderSize.Width} should be exactly equal to {nameof(given)} value {givenContent.RenderSize.Width}");
				sutContent.RenderSize.Height.Should().Be(givenContent.RenderSize.Height, $"Content RenderSize.Height {sutContent.RenderSize.Height} should be exactly equal to {nameof(given)} value {givenContent.RenderSize.Height}");

				expectedWidthDifference = contentMargin.Left + contentMargin.Right + svPadding.Left + svPadding.Right;
				expectedHeightDifference = contentMargin.Top + contentMargin.Bottom + svPadding.Top + svPadding.Bottom;
				sut.ScrollableWidth.Should().BeApproximately(given.ScrollableWidth + expectedWidthDifference, maximumValidDifference, $"ScrollableWidth should be the {nameof(given)} value {given.ScrollableWidth} increased by Content.Margin Left + Right ScrollViewer.Padding Left + Right {Math.Abs(expectedWidthDifference)}");
				sut.ScrollableHeight.Should().BeApproximately(given.ScrollableHeight + expectedHeightDifference, maximumValidDifference, $"ScrollableHeight should be the {nameof(given)} value {given.ScrollableHeight} increaded by Content.Margin Top + Bottom + ScrollViewer.Padding Top + Bottom {Math.Abs(expectedHeightDifference)}");

				sut.HorizontalOffset.Should().BeApproximately(sut.ScrollableWidth, maximumValidDifference, $"HorizontalOffset should be {nameof(sut.ScrollableWidth)} value {sut.ScrollableWidth}");
				sut.VerticalOffset.Should().BeApproximately(sut.ScrollableHeight, maximumValidDifference, $"VerticalOffset should be {nameof(sut.ScrollableHeight)} value {sut.ScrollableHeight}");
				sut.ChangeView(0, 0, null, true);
				await WindowHelper.WaitFor(() => sut.HorizontalOffset == 0, 1000, $"Waiting for {nameof(sut.HorizontalOffset)} to change to 0.");
				await WindowHelper.WaitFor(() => sut.VerticalOffset == 0, 1000, $"Waiting for {nameof(sut.VerticalOffset)} to change to 0.");
			}
		}
	}
}
