using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

using UITests.Shared.Helpers;
using Windows.UI.Text;

namespace UITests.Windows_UI_Xaml_Controls.ScrollViewerTests
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ScrollViewerContentExtentData : BindableBase
	{
		// We consider a one pixel difference value to be a valid difference due to internal layout and rounding differences on various platforms.
		private const int _maximumValidDifference = 1;
		public static bool InvalidDifferenceCheck(double x, double y) => Math.Abs(x - y) > _maximumValidDifference;
		public static int TestPassedValue(double x, double y) => InvalidDifferenceCheck(x, y) ? 0 : 1;
		public bool? InvalidDifferenceBool(double x, double y) => InvalidDifferenceCheck(x, y);
		public string DifferenceCheckResultText(double x, double y) => InvalidDifferenceBool(x, y) is true ? "Failed" : "Passed";
		public FontWeight DifferenceCheckResultFontWeight(double x, double y) => InvalidDifferenceBool(x, y) is true ? FontWeights.Bold : FontWeights.Normal;

		public void UpdateTestsPassedFailedValues(int numberOfTests, int passedTestsCount)
		{
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

		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
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

		private const int _numberOfDigitsForRounding = 3;
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
