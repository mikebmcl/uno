using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

using UITests.Shared.Helpers;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace UITests.Windows_UI_Xaml_Controls.ScrollViewerTests
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ScrollViewerPaddingContentMarginViewModel : BindableBase
	{
		public ScrollViewerPaddingContentMarginViewModel(string displayName)
		{
			DisplayName = displayName;
		}
		// We consider a one pixel difference value to be a valid difference due to internal layout and rounding differences on various platforms.
		private const int _maximumValidDifference = 1;
		public static bool InvalidDifferenceCheck(double x, double y) => Math.Abs(x - y) > _maximumValidDifference;
		public static int TestPassedValue(double x, double y) => InvalidDifferenceCheck(x, y) ? 0 : 1;
		public bool? InvalidDifferenceBool(double x, double y) => InvalidDifferenceCheck(x, y);
		public string DifferenceCheckResultText(double x, double y) => InvalidDifferenceBool(x, y) is true ? "Failed" : "Passed";
		public FontWeight DifferenceCheckResultFontWeight(double x, double y) => InvalidDifferenceBool(x, y) is true ? FontWeights.Bold : FontWeights.Normal;

		private string _dblFormat = "F";
		private System.Globalization.CultureInfo _cultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
		private int _numberOfDigitsForRounding = 3;
		public int NumberOfDigitsForRounding
		{
			get => _numberOfDigitsForRounding;
			set
			{
				if (_numberOfDigitsForRounding != value)
				{
					_numberOfDigitsForRounding = value;
					RaisePropertyChanged();
				}
			}
		}
		private bool _scrollTestBegan;
		private bool _scrollTestHorizontalScrollFirstScrollComplete;
		private bool _scrollTestHorizontalScrollIsReady;
		private bool _scrollTestVerticalScrollIsReady;
		private bool _scrollTestComplete;
		private bool _scrollTestIsRunning;
		private bool _scrollTestResetRequested;
		private void ResetScrollTestValues()
		{
			_scrollTestHorizontalScrollFirstScrollComplete = false;
			_scrollTestHorizontalScrollIsReady = false;
			_scrollTestVerticalScrollIsReady = false;
			_scrollTestComplete = false;
			HorizontalOffsetAfterScrollTest = 0;
			VerticalOffsetAfterScrollTest = 0;
		}

		public void BeginScrollTest(ScrollViewer sv)
		{
			var name = sv.Name;
			var halfScrollableWidth = sv.ScrollableWidth / 2;
			ResetScrollTestValues();
			if (_scrollTestIsRunning)
			{
				if (_scrollTestResetRequested && (sv.ScrollableWidth == 0 || sv.ScrollableHeight == 0))
				{
					return;
				}
				_scrollTestResetRequested = true;
				// We don't always get an exact match when the HorizontalOffset is theoretically equal to the ScrollableWidth on Android (maybe other platforms too), so we allow a less than 1 pixel discrepancy rather than checking for exact equality
				if (Math.Abs(sv.HorizontalOffset - sv.ScrollableWidth) < 1)
				{
					_ = sv.ChangeView(0, 0, null, true);
				}
				else
				{
					_scrollTestBegan = true;
					_ = sv.ChangeView(halfScrollableWidth, 0, null, true);
				}
				return;
			}
			_scrollTestIsRunning = true;
			_scrollTestBegan = true;
			_ = sv.ChangeView(halfScrollableWidth, 0, null, true);
		}

		public void UpdateScrollTest(ScrollViewer noMarginAndNoPadding, ScrollViewer sv)
		{
			var name = sv.Name;
			var halfScrollableWidth = sv.ScrollableWidth / 2;
			if (_scrollTestResetRequested)
			{
				ResetScrollTestValues();
				_scrollTestResetRequested = false;
				_scrollTestIsRunning = true;
				if (sv.HorizontalOffset > 0)
				{
					_ = sv.ChangeView(0, 0, null, true);
				}
				else
				{
					_scrollTestBegan = true;
					_ = sv.ChangeView(halfScrollableWidth, 0, null, true);
				}
				return;
			}
			if (_scrollTestIsRunning)
			{
				if (_scrollTestIsRunning && !_scrollTestBegan)
				{
					if (sv.HorizontalOffset > 0)
					{
						ResetScrollTestValues();
						_ = sv.ChangeView(0, 0, null, true);
					}
					else
					{
						_scrollTestBegan = true;
						_scrollTestHorizontalScrollFirstScrollComplete = false;
						_scrollTestVerticalScrollIsReady = false;
						_ = sv.ChangeView(halfScrollableWidth, 0, null, true);
					}
					return;
				}
				if (_scrollTestBegan && !_scrollTestHorizontalScrollFirstScrollComplete && sv.VerticalOffset == 0)
				{
					if (sv.HorizontalOffset == 0)
					{
						_scrollTestBegan = true;
						_scrollTestVerticalScrollIsReady = false;
						_ = sv.ChangeView(halfScrollableWidth, 0, null, true);
						return;
					}
					else
					{
						_scrollTestHorizontalScrollFirstScrollComplete = true;
						_scrollTestVerticalScrollIsReady = false;
						_ = sv.ChangeView(null, sv.ScrollableHeight, null, true);
						return;
					}
				}
				if (_scrollTestHorizontalScrollFirstScrollComplete && !_scrollTestVerticalScrollIsReady && sv.HorizontalOffset > 0)
				{
					_scrollTestVerticalScrollIsReady = true;
					HorizontalOffsetAfterScrollTest = sv.HorizontalOffset;
					_ = sv.ChangeView(sv.ScrollableWidth, null, null, true);
					return;
				}
				if (_scrollTestHorizontalScrollFirstScrollComplete && _scrollTestVerticalScrollIsReady && !_scrollTestHorizontalScrollIsReady && sv.VerticalOffset > 0 && sv.HorizontalOffset > 0)
				{
					VerticalOffsetAfterScrollTest = sv.VerticalOffset;
					_ = sv.ChangeView(0, 0, null, true);
					return;
				}
				if (_scrollTestHorizontalScrollFirstScrollComplete && _scrollTestVerticalScrollIsReady && !_scrollTestHorizontalScrollIsReady && sv.VerticalOffset == 0 && sv.HorizontalOffset == 0)
				{
					_ = sv.ChangeView(sv.ScrollableWidth, null, null, true);
					return;
				}
				if (_scrollTestHorizontalScrollFirstScrollComplete && _scrollTestVerticalScrollIsReady && !_scrollTestHorizontalScrollIsReady && sv.VerticalOffset == 0 && sv.HorizontalOffset > 0)
				{
					_scrollTestHorizontalScrollIsReady = true;
					_ = sv.ChangeView(null, sv.ScrollableHeight, null, true);
					return;
				}
				if (_scrollTestHorizontalScrollFirstScrollComplete && _scrollTestVerticalScrollIsReady && _scrollTestHorizontalScrollIsReady && sv.HorizontalOffset > 0 && !_scrollTestComplete)
				{
					HorizontalOffsetAfterScrollTest = sv.HorizontalOffset;
					_scrollTestComplete = true;
					_ = sv.ChangeView(0, 0, null, true);
					return;
				}
				UpdateTestsPassedFailedValues(noMarginAndNoPadding, sv);
				_scrollTestIsRunning = false;
			}
		}

		private void UpdateData(ScrollViewer sv)
		{
			var numberOfDigitsForRounding = _numberOfDigitsForRounding;
			var svContent = sv.Content as FrameworkElement;
			ScrollViewerName = sv.Name;
			ContentMargin = svContent?.Margin ?? default;
			ScrollViewerPadding = sv.Padding;

			ContentActualHeight = $"Content ActualHeight: {(svContent?.ActualHeight ?? double.NaN).ToString(_dblFormat, _cultureInfo)}";
			ContentActualHeightValue = Math.Round(svContent?.ActualHeight ?? double.NaN, numberOfDigitsForRounding);

			ContentActualWidth = $"Content ActualWidth: {(svContent?.ActualWidth ?? double.NaN).ToString(_dblFormat, _cultureInfo)}";
			ContentActualWidthValue = Math.Round(svContent?.ActualWidth ?? double.NaN, numberOfDigitsForRounding);

			SVActualHeight = $"ScrollViewer ActualHeight: {sv.ActualHeight.ToString(_dblFormat, _cultureInfo)}";
			SVActualHeightValue = Math.Round(sv.ActualHeight, numberOfDigitsForRounding);

			SVActualWidth = $"ScrollViewer ActualWidth: {sv.ActualWidth.ToString(_dblFormat, _cultureInfo)}";
			SVActualWidthValue = Math.Round(sv.ActualWidth, numberOfDigitsForRounding);

			ExtentHeight = $"ExtentHeight: {sv.ExtentHeight.ToString(_dblFormat, _cultureInfo)}";
			ExtentHeightValue = Math.Round(sv.ExtentHeight, numberOfDigitsForRounding);

			ExtentWidth = $"ExtentWidth: {sv.ExtentWidth.ToString(_dblFormat, _cultureInfo)}";
			ExtentWidthValue = Math.Round(sv.ExtentWidth, numberOfDigitsForRounding);

			ViewportHeight = $"ViewportHeight: {sv.ViewportHeight.ToString(_dblFormat, _cultureInfo)}";
			ViewportHeightValue = Math.Round(sv.ViewportHeight, numberOfDigitsForRounding);

			ViewportWidth = $"ViewportWidth: {sv.ViewportWidth.ToString(_dblFormat, _cultureInfo)}";
			ViewportWidthValue = Math.Round(sv.ViewportWidth, numberOfDigitsForRounding);

			ScrollableHeight = $"ScrollableHeight: {sv.ScrollableHeight.ToString(_dblFormat, _cultureInfo)}";
			ScrollableHeightValue = Math.Round(sv.ScrollableHeight, numberOfDigitsForRounding);

			ScrollableWidth = $"ScrollableWidth: {sv.ScrollableWidth.ToString(_dblFormat, _cultureInfo)}";
			ScrollableWidthValue = Math.Round(sv.ScrollableWidth, numberOfDigitsForRounding);

			HorizontalOffsetValue = Math.Round(sv.HorizontalOffset, numberOfDigitsForRounding);
			//HorizontalOffsetAfterScrollTest = Math.Round(horizontalScrollTestResult);

			VerticalOffsetValue = Math.Round(sv.VerticalOffset, numberOfDigitsForRounding);
			//VerticalOffsetAfterScrollTest = Math.Round(verticalScrollTestResult);
		}

		public void UpdateTestsPassedFailedValues(ScrollViewer noMarginAndNoPadding, ScrollViewer sv)
		{
			UpdateData(sv);

			var numberOfTests = 0;
			var passedTestsCount = 0;
			var fallbackDoubleValue = double.NaN;
			var numberOfDigitsForRounding = _numberOfDigitsForRounding;

			SVActualWidthExpectedDifference = 0;
			SVActualWidthActualDifference = Math.Round(noMarginAndNoPadding.ActualWidth - SVActualWidthValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(SVActualWidthExpectedDifference, SVActualWidthActualDifference);

			SVActualHeightExpectedDifference = 0;
			SVActualHeightActualDifference = Math.Round(noMarginAndNoPadding.ActualHeight - SVActualHeightValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(SVActualHeightExpectedDifference, SVActualHeightActualDifference);

			ViewportWidthExpectedDifference = ScrollViewerPadding.Left + ScrollViewerPadding.Right;
			ViewportWidthActualDifference = Math.Round(noMarginAndNoPadding.ViewportWidth - ViewportWidthValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ViewportWidthExpectedDifference, ViewportWidthActualDifference);

			ViewportHeightExpectedDifference = ScrollViewerPadding.Top + ScrollViewerPadding.Bottom;
			ViewportHeightActualDifference = Math.Round(noMarginAndNoPadding.ViewportHeight - ViewportHeightValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ViewportHeightExpectedDifference, ViewportHeightActualDifference);

			ExtentWidthExpectedDifference = -(ContentMargin.Left + ContentMargin.Right);
			ExtentWidthActualDifference = Math.Round(noMarginAndNoPadding.ExtentWidth - ExtentWidthValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ExtentWidthExpectedDifference, ExtentWidthActualDifference);

			ExtentHeightExpectedDifference = -(ContentMargin.Top + ContentMargin.Bottom);
			ExtentHeightActualDifference = Math.Round(noMarginAndNoPadding.ExtentHeight - ExtentHeightValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ExtentHeightExpectedDifference, ExtentHeightActualDifference);

			ContentActualWidthExpectedDifference = 0;
			var noMarginAndNoPaddingContent = noMarginAndNoPadding.Content as FrameworkElement;
			ContentActualWidthActualDifference = Math.Round((noMarginAndNoPaddingContent?.ActualWidth ?? fallbackDoubleValue) - ContentActualWidthValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ContentActualWidthExpectedDifference, ContentActualWidthActualDifference);

			ContentActualHeightExpectedDifference = 0;
			ContentActualHeightActualDifference = Math.Round((noMarginAndNoPaddingContent?.ActualHeight ?? fallbackDoubleValue) - ContentActualHeightValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ContentActualHeightExpectedDifference, ContentActualHeightActualDifference);

			ScrollableWidthExpectedDifference = -(ContentMargin.Left + ContentMargin.Right + ScrollViewerPadding.Left + ScrollViewerPadding.Right);
			ScrollableWidthActualDifference = Math.Round(noMarginAndNoPadding.ScrollableWidth - ScrollableWidthValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ScrollableWidthExpectedDifference, ScrollableWidthActualDifference);

			ScrollableHeightExpectedDifference = -(ContentMargin.Top + ContentMargin.Bottom + ScrollViewerPadding.Top + ScrollViewerPadding.Bottom);
			ScrollableHeightActualDifference = Math.Round(noMarginAndNoPadding.ScrollableHeight - ScrollableHeightValue, numberOfDigitsForRounding);
			numberOfTests++;
			passedTestsCount += TestPassedValue(ScrollableHeightExpectedDifference, ScrollableHeightActualDifference);

			numberOfTests += 2;
			passedTestsCount += TestPassedValue(sv.ScrollableWidth, HorizontalOffsetAfterScrollTest);
			passedTestsCount += TestPassedValue(sv.ScrollableHeight, VerticalOffsetAfterScrollTest);
			NumberOfTests = numberOfTests;
			PassedTestsCount = passedTestsCount;

			TestsPassedFailedCountText = $"{passedTestsCount} of {numberOfTests} Tests Passed";
		}

		public int NumberOfTests { get; set; }
		public int PassedTestsCount { get; set; }

		private string _testsPassedFailedCountText;
		public string TestsPassedFailedCountText
		{
			get => _testsPassedFailedCountText;
			set
			{
				if (_testsPassedFailedCountText != value)
				{
					_testsPassedFailedCountText = value;
					RaisePropertyChanged();
				}
			}
		}

		public static string ThicknessAsString(Thickness t) => $"{{ L:{t.Left} T:{t.Top} R:{t.Right} B:{t.Bottom} }}";

		private string _displayName;
		public string DisplayName
		{
			get => _displayName;
			set
			{
				if (_displayName != value)
				{
					_displayName = value;
					RaisePropertyChanged();
				}
			}
		}
		private string _scrollViewerName;
		public string ScrollViewerName
		{
			get => _scrollViewerName;
			set
			{
				if (_scrollViewerName != value)
				{
					_scrollViewerName = value;
					RaisePropertyChanged();
				}
			}
		}
		public string ScrollViewerPaddingName => nameof(ScrollViewerPadding) + ":";
		private Thickness _scrollViewerPadding;
		public Thickness ScrollViewerPadding
		{
			get => _scrollViewerPadding;
			set
			{
				if (_scrollViewerPadding != value)
				{
					_scrollViewerPadding = value;
					RaisePropertyChanged();
				}
			}
		}

		public string ContentMarginName => nameof(ContentMargin) + ":";
		private Thickness _contentMargin;
		public Thickness ContentMargin
		{
			get => _contentMargin;
			set
			{
				if (_contentMargin != value)
				{
					_contentMargin = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _svActualWidth;
		public string SVActualWidth
		{
			get => _svActualWidth;
			set
			{
				if (_svActualWidth != value)
				{
					_svActualWidth = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualWidthValue;
		public double SVActualWidthValue
		{
			get => _svActualWidthValue;
			set
			{
				if (_svActualWidthValue != value)
				{
					_svActualWidthValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualWidthExpectedDifference;
		public double SVActualWidthExpectedDifference
		{
			get => _svActualWidthExpectedDifference;
			set
			{
				if (_svActualWidthExpectedDifference != value)
				{
					_svActualWidthExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualWidthActualDifference;
		public double SVActualWidthActualDifference
		{
			get => _svActualWidthActualDifference;
			set
			{
				if (_svActualWidthActualDifference != value)
				{
					_svActualWidthActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _svActualHeight;
		public string SVActualHeight
		{
			get => _svActualHeight;
			set
			{
				if (_svActualHeight != value)
				{
					_svActualHeight = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualHeightValue;
		public double SVActualHeightValue
		{
			get => _svActualHeightValue;
			set
			{
				if (_svActualHeightValue != value)
				{
					_svActualHeightValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualHeightExpectedDifference;
		public double SVActualHeightExpectedDifference
		{
			get => _svActualHeightExpectedDifference;
			set
			{
				if (_svActualHeightExpectedDifference != value)
				{
					_svActualHeightExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _svActualHeightActualDifference;
		public double SVActualHeightActualDifference
		{
			get => _svActualHeightActualDifference;
			set
			{
				if (_svActualHeightActualDifference != value)
				{
					_svActualHeightActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _viewPortWidth;
		public string ViewportWidth
		{
			get => _viewPortWidth;
			set
			{
				if (_viewPortWidth != value)
				{
					_viewPortWidth = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortWidthValue;
		public double ViewportWidthValue
		{
			get => _viewPortWidthValue;
			set
			{
				if (_viewPortWidthValue != value)
				{
					_viewPortWidthValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortWidthExpectedDifference;
		public double ViewportWidthExpectedDifference
		{
			get => _viewPortWidthExpectedDifference;
			set
			{
				if (_viewPortWidthExpectedDifference != value)
				{
					_viewPortWidthExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortWidthActualDifference;
		public double ViewportWidthActualDifference
		{
			get => _viewPortWidthActualDifference;
			set
			{
				if (_viewPortWidthActualDifference != value)
				{
					_viewPortWidthActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _viewPortHeight;
		public string ViewportHeight
		{
			get => _viewPortHeight;
			set
			{
				if (_viewPortHeight != value)
				{
					_viewPortHeight = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortHeightValue;
		public double ViewportHeightValue
		{
			get => _viewPortHeightValue;
			set
			{
				if (_viewPortHeightValue != value)
				{
					_viewPortHeightValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortHeightExpectedDifference;
		public double ViewportHeightExpectedDifference
		{
			get => _viewPortHeightExpectedDifference;
			set
			{
				if (_viewPortHeightExpectedDifference != value)
				{
					_viewPortHeightExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _viewPortHeightActualDifference;
		public double ViewportHeightActualDifference
		{
			get => _viewPortHeightActualDifference;
			set
			{
				if (_viewPortHeightActualDifference != value)
				{
					_viewPortHeightActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _extentWidth;
		public string ExtentWidth
		{
			get => _extentWidth;
			set
			{
				if (_extentWidth != value)
				{
					_extentWidth = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentWidthValue;
		public double ExtentWidthValue
		{
			get => _extentWidthValue;
			set
			{
				if (_extentWidthValue != value)
				{
					_extentWidthValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentWidthExpectedDifference;
		public double ExtentWidthExpectedDifference
		{
			get => _extentWidthExpectedDifference;
			set
			{
				if (_extentWidthExpectedDifference != value)
				{
					_extentWidthExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentWidthActualDifference;
		public double ExtentWidthActualDifference
		{
			get => _extentWidthActualDifference;
			set
			{
				if (_extentWidthActualDifference != value)
				{
					_extentWidthActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _extentHeight;
		public string ExtentHeight
		{
			get => _extentHeight;
			set
			{
				if (_extentHeight != value)
				{
					_extentHeight = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentHeightValue;
		public double ExtentHeightValue
		{
			get => _extentHeightValue;
			set
			{
				if (_extentHeightValue != value)
				{
					_extentHeightValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentHeightExpectedDifference;
		public double ExtentHeightExpectedDifference
		{
			get => _extentHeightExpectedDifference;
			set
			{
				if (_extentHeightExpectedDifference != value)
				{
					_extentHeightExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _extentHeightActualDifference;
		public double ExtentHeightActualDifference
		{
			get => _extentHeightActualDifference;
			set
			{
				if (_extentHeightActualDifference != value)
				{
					_extentHeightActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _contentActualWidth;
		public string ContentActualWidth
		{
			get => _contentActualWidth;
			set
			{
				if (_contentActualWidth != value)
				{
					_contentActualWidth = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualWidthValue;
		public double ContentActualWidthValue
		{
			get => _contentActualWidthValue;
			set
			{
				if (_contentActualWidthValue != value)
				{
					_contentActualWidthValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualWidthExpectedDifference;
		public double ContentActualWidthExpectedDifference
		{
			get => _contentActualWidthExpectedDifference;
			set
			{
				if (_contentActualWidthExpectedDifference != value)
				{
					_contentActualWidthExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualWidthActualDifference;
		public double ContentActualWidthActualDifference
		{
			get => _contentActualWidthActualDifference;
			set
			{
				if (_contentActualWidthActualDifference != value)
				{
					_contentActualWidthActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _contentActualHeight;
		public string ContentActualHeight
		{
			get => _contentActualHeight;
			set
			{
				if (_contentActualHeight != value)
				{
					_contentActualHeight = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualHeightValue;
		public double ContentActualHeightValue
		{
			get => _contentActualHeightValue;
			set
			{
				if (_contentActualHeightValue != value)
				{
					_contentActualHeightValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualHeightExpectedDifference;
		public double ContentActualHeightExpectedDifference
		{
			get => _contentActualHeightExpectedDifference;
			set
			{
				if (_contentActualHeightExpectedDifference != value)
				{
					_contentActualHeightExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _contentActualHeightActualDifference;
		public double ContentActualHeightActualDifference
		{
			get => _contentActualHeightActualDifference;
			set
			{
				if (_contentActualHeightActualDifference != value)
				{
					_contentActualHeightActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _scrollableWidth;
		public string ScrollableWidth
		{
			get => _scrollableWidth;
			set
			{
				if (_scrollableWidth != value)
				{
					_scrollableWidth = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableWidthValue;
		public double ScrollableWidthValue
		{
			get => _scrollableWidthValue;
			set
			{
				if (_scrollableWidthValue != value)
				{
					_scrollableWidthValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableWidthExpectedDifference;
		public double ScrollableWidthExpectedDifference
		{
			get => _scrollableWidthExpectedDifference;
			set
			{
				if (_scrollableWidthExpectedDifference != value)
				{
					_scrollableWidthExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableWidthActualDifference;
		public double ScrollableWidthActualDifference
		{
			get => _scrollableWidthActualDifference;
			set
			{
				if (_scrollableWidthActualDifference != value)
				{
					_scrollableWidthActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private string _scrollableHeight;
		public string ScrollableHeight
		{
			get => _scrollableHeight;
			set
			{
				if (_scrollableHeight != value)
				{
					_scrollableHeight = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableHeightValue;
		public double ScrollableHeightValue
		{
			get => _scrollableHeightValue;
			set
			{
				if (_scrollableHeightValue != value)
				{
					_scrollableHeightValue = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableHeightExpectedDifference;
		public double ScrollableHeightExpectedDifference
		{
			get => _scrollableHeightExpectedDifference;
			set
			{
				if (_scrollableHeightExpectedDifference != value)
				{
					_scrollableHeightExpectedDifference = value;
					RaisePropertyChanged();
				}
			}
		}
		private double _scrollableHeightActualDifference;
		public double ScrollableHeightActualDifference
		{
			get => _scrollableHeightActualDifference;
			set
			{
				if (_scrollableHeightActualDifference != value)
				{
					_scrollableHeightActualDifference = value;
					RaisePropertyChanged();
				}
			}
		}

		private double _horizontalOffsetValue;
		public double HorizontalOffsetValue
		{
			get => _horizontalOffsetValue;
			set
			{
				if (_horizontalOffsetValue != Math.Round(value, _numberOfDigitsForRounding))
				{
					_horizontalOffsetValue = Math.Round(value, _numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}
		private double _horizontalOffsetAfterScrollTest;
		public double HorizontalOffsetAfterScrollTest
		{
			get => _horizontalOffsetAfterScrollTest;
			set
			{
				if (_horizontalOffsetAfterScrollTest != Math.Round(value, _numberOfDigitsForRounding))
				{
					_horizontalOffsetAfterScrollTest = Math.Round(value, _numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}

		private double _verticalOffsetValue;
		public double VerticalOffsetValue
		{
			get => _verticalOffsetValue;
			set
			{
				if (_verticalOffsetValue != Math.Round(value, _numberOfDigitsForRounding))
				{
					_verticalOffsetValue = Math.Round(value, _numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}
		private double _verticalOffsetAfterScrollTest;
		public double VerticalOffsetAfterScrollTest
		{
			get => _verticalOffsetAfterScrollTest;
			set
			{
				if (_verticalOffsetAfterScrollTest != Math.Round(value, _numberOfDigitsForRounding))
				{
					_verticalOffsetAfterScrollTest = Math.Round(value, _numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}
	}
}
