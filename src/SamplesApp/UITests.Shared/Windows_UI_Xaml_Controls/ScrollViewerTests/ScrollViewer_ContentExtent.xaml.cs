using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using UITests.Shared.Helpers;
using System.Collections.ObjectModel;
#if HAS_UNO
using Uno.Foundation.Logging;
#else
using Microsoft.Extensions.Logging;
using Uno.Logging;
#endif

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UITests.Windows_UI_Xaml_Controls.ScrollViewerTests
{
	[Sample("ScrollViewer", Description = "Checks ScrollViewer Margin and Content Padding layout.")]
	public sealed partial class ScrollViewer_ContentExtent : Page, INotifyPropertyChanged
	{
		public ScrollViewer_ContentExtent()
		{
			this.InitializeComponent();

			// Create with three empty entries because we want the data order to match the order of the ScrollViewers
			ScrollViewerData = new ObservableCollection<ScrollViewerContentExtentData>(new ScrollViewerContentExtentData[] { new ScrollViewerContentExtentData(), new ScrollViewerContentExtentData(), new ScrollViewerContentExtentData() });

			NeitherStackPanel.SizeChanged += NeitherStackPanel_SizeChanged;
			NeitherScrollViewer.SizeChanged += NeitherScrollViewer_SizeChanged;
			ContentMarginStackPanel.SizeChanged += ContentMarginStackPanel_SizeChanged;
			ContentMarginScrollViewer.SizeChanged += ContentMarginScrollViewer_SizeChanged;
			SVPaddingStackPanel.SizeChanged += SVPaddingStackPanel_SizeChanged;
			SVPaddingScrollViewer.SizeChanged += SVPaddingScrollViewer_SizeChanged;
			BothStackPanel.SizeChanged += BothStackPanel_SizeChanged;
			BothScrollViewer.SizeChanged += BothScrollViewer_SizeChanged;
			// Note: We need to use LayoutUpdated events because the Viewport* and Extent* data isn't ready (except in UWP) when SizeChanged fires for the first time and we don't want to have to resize the window just to get the data.
			NeitherScrollViewer.LayoutUpdated += NeitherScrollViewer_LayoutUpdated;
			ContentMarginScrollViewer.LayoutUpdated += ContentMarginScrollViewer_LayoutUpdated;
			SVPaddingScrollViewer.LayoutUpdated += SVPaddingScrollViewer_LayoutUpdated;
			BothScrollViewer.LayoutUpdated += BothScrollViewer_LayoutUpdated;

			ContentMarginScrollViewer.ViewChanged += ContentMarginScrollViewer_ViewChanged;
			SVPaddingScrollViewer.ViewChanged += SVPaddingScrollViewer_ViewChanged;
			BothScrollViewer.ViewChanged += BothScrollViewer_ViewChanged;
			NeitherScrollViewer.ViewChanged += NeitherScrollViewer_ViewChanged;
			Loaded += ScrollViewer_ContentExtent_Loaded;
		}

		private void RunTestsButtonClickHandler(object sender, RoutedEventArgs e)
		{
			_neitherScrollViewerScrollTestComplete = false;
			_contentMarginScrollViewerScrollTestComplete = false;
			_svPaddingScrollViewerScrollTestComplete = false;
			_bothScrollViewerScrollTestComplete = false;
			ScrollViewer_ContentExtent_Loaded(sender, e);
		}

		private void ScrollViewer_ContentExtent_Loaded(object sender, RoutedEventArgs e)
		{
			NeitherScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(NeitherScrollViewer);
			ContentMarginScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(ContentMarginScrollViewer);
			SVPaddingScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(SVPaddingScrollViewer);
			BothScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(BothScrollViewer);
		}

		private bool _neitherScrollViewerScrollTestComplete;
		private double _neitherScrollViewerScrollTestHorizontalResult;
		private double _neitherScrollViewerScrollTestVerticalResult;
		private void NeitherScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = NeitherScrollViewerContentExtentData;
			var sv = NeitherScrollViewer;
			if (data != null)
			{
				if (!_neitherScrollViewerScrollTestComplete && sv.HorizontalOffset != 0 && sv.VerticalOffset != 0)
				{
					FinishRunScrollToOffsetsTest(sv, data);
				}
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private bool _contentMarginScrollViewerScrollTestComplete;
		private double _contentMarginScrollViewerScrollTestHorizontalResult;
		private double _contentMarginScrollViewerScrollTestVerticalResult;
		private void ContentMarginScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = ContentMarginScrollViewerContentExtentData;
			var sv = ContentMarginScrollViewer;
			if (data != null)
			{
				if (!_contentMarginScrollViewerScrollTestComplete && sv.HorizontalOffset != 0 && sv.VerticalOffset != 0)
				{
					FinishRunScrollToOffsetsTest(sv, data);
					PopulateExpectedAndActualValuesForScrollViewerContentExtentData(GetContentExtentDataForScrollViewer(NeitherScrollViewer), data, sv.Padding, ((sv.Content as StackPanel)?.Margin ?? default));
				}
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private bool _svPaddingScrollViewerScrollTestComplete;
		private double _svPaddingScrollViewerScrollTestHorizontalResult;
		private double _svPaddingScrollViewerScrollTestVerticalResult;
		private void SVPaddingScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = SVPaddingScrollViewerContentExtentData;
			var sv = SVPaddingScrollViewer;
			if (data != null)
			{
				if (!_svPaddingScrollViewerScrollTestComplete && sv.HorizontalOffset != 0 && sv.VerticalOffset != 0)
				{
					FinishRunScrollToOffsetsTest(sv, data);
					PopulateExpectedAndActualValuesForScrollViewerContentExtentData(GetContentExtentDataForScrollViewer(NeitherScrollViewer), data, sv.Padding, ((sv.Content as StackPanel)?.Margin ?? default));
				}
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private bool _bothScrollViewerScrollTestComplete;
		private double _bothScrollViewerScrollTestHorizontalResult;
		private double _bothScrollViewerScrollTestVerticalResult;
		private void BothScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = BothScrollViewerContentExtentData;
			var sv = BothScrollViewer;
			if (data != null)
			{
				if (!_bothScrollViewerScrollTestComplete && sv.HorizontalOffset != 0 && sv.VerticalOffset != 0)
				{
					FinishRunScrollToOffsetsTest(sv, data);
					PopulateExpectedAndActualValuesForScrollViewerContentExtentData(GetContentExtentDataForScrollViewer(NeitherScrollViewer), data, sv.Padding, ((sv.Content as StackPanel)?.Margin ?? default));
				}
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

#pragma warning disable CS0109
#if HAS_UNO
		private new readonly Logger _log = Uno.Foundation.Logging.LogExtensionPoint.Log(typeof(ScrollViewer));
#else
		private static readonly ILogger _log = Uno.Extensions.LogExtensionPoint.Log(typeof(ScrollViewer));
#endif
#pragma warning restore CS0109

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private Thickness _scrollViewerBorderThickness = new Thickness(1);
		public Thickness ScrollViewerBorderThickness
		{
			get => _scrollViewerBorderThickness;
			set
			{
				if (_scrollViewerBorderThickness != value)
				{
					_scrollViewerBorderThickness = value;
					RaisePropertyChanged();
				}
			}
		}
		private Thickness _contentBorderThickness = new Thickness(2);
		public Thickness ContentBorderThickness
		{
			get => _contentBorderThickness;
			set
			{
				if (_contentBorderThickness != value)
				{
					_contentBorderThickness = value;
					RaisePropertyChanged();
				}
			}
		}

		public string ScrollViewerPaddingName => nameof(ScrollViewerPadding);
		private Thickness _scrollViewerPadding = new Thickness(16);
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

		public string ContentMarginName => nameof(ContentMargin);
		private Thickness _contentMargin = new Thickness(10);
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

		private ScrollBarVisibility _verticalScrollBarVisibility = ScrollBarVisibility.Visible;
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get => _verticalScrollBarVisibility;
			set
			{
				if (_verticalScrollBarVisibility != value)
				{
					_verticalScrollBarVisibility = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollBarVisibility _horizontalScrollBarVisibility = ScrollBarVisibility.Visible;
		public ScrollBarVisibility HorizontalScrollBarVisibility
		{
			get => _horizontalScrollBarVisibility;
			set
			{
				if (_horizontalScrollBarVisibility != value)
				{
					_horizontalScrollBarVisibility = value;
					RaisePropertyChanged();
				}
			}
		}

		public ObservableCollection<ScrollViewerContentExtentData> ScrollViewerData { get; set; }
		string _dblFormat = "F";
		System.Globalization.CultureInfo _cultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
		private ScrollViewerContentExtentData GetContentExtentDataForScrollViewer(ScrollViewer sv)
		{
			Thickness contentMargin;
			Thickness scrollViewerPadding;
			double horizontalScrollTestResult = 0;
			double verticalScrollTestResult = 0;
			switch (sv.Name)
			{
				case nameof(ContentMarginScrollViewer):
					contentMargin = ContentMargin;
					scrollViewerPadding = new Thickness();
					horizontalScrollTestResult = _contentMarginScrollViewerScrollTestHorizontalResult;
					verticalScrollTestResult = _contentMarginScrollViewerScrollTestVerticalResult;
					break;
				case nameof(SVPaddingScrollViewer):
					contentMargin = new Thickness();
					scrollViewerPadding = ScrollViewerPadding;
					horizontalScrollTestResult = _svPaddingScrollViewerScrollTestHorizontalResult;
					verticalScrollTestResult = _svPaddingScrollViewerScrollTestVerticalResult;
					break;
				case nameof(BothScrollViewer):
					contentMargin = ContentMargin;
					scrollViewerPadding = ScrollViewerPadding;
					horizontalScrollTestResult = _bothScrollViewerScrollTestHorizontalResult;
					verticalScrollTestResult = _bothScrollViewerScrollTestVerticalResult;
					break;
				case nameof(NeitherScrollViewer):
					contentMargin = new Thickness();
					scrollViewerPadding = new Thickness();
					horizontalScrollTestResult = _neitherScrollViewerScrollTestHorizontalResult;
					verticalScrollTestResult = _neitherScrollViewerScrollTestVerticalResult;
					break;
				default:
					//_log.Debug($"Unexpected data with name '{scrollViewerName ?? "(null)"}");
					contentMargin = new Thickness(double.NaN);
					scrollViewerPadding = new Thickness(double.NaN);
					break;
			}
			const int numberOfDigits = 3;
			var svContent = sv.Content;
			return new ScrollViewerContentExtentData()
			{
				ScrollViewerName = sv.Name,
				ContentMargin = contentMargin,
				ScrollViewerPadding = scrollViewerPadding,

				ContentActualHeight = $"Content ActualHeight: {((svContent as FrameworkElement)?.ActualHeight ?? double.NaN).ToString(_dblFormat, _cultureInfo)}",
				ContentActualHeightValue = Math.Round((svContent as FrameworkElement)?.ActualHeight ?? double.NaN, 3),

				ContentActualWidth = $"Content ActualWidth: {((svContent as FrameworkElement)?.ActualWidth ?? double.NaN).ToString(_dblFormat, _cultureInfo)}",
				ContentActualWidthValue = Math.Round((svContent as FrameworkElement)?.ActualWidth ?? double.NaN, numberOfDigits),

				SVActualHeight = $"ScrollViewer ActualHeight: {sv.ActualHeight.ToString(_dblFormat, _cultureInfo)}",
				SVActualHeightValue = Math.Round(sv.ActualHeight, numberOfDigits),

				SVActualWidth = $"ScrollViewer ActualWidth: {sv.ActualWidth.ToString(_dblFormat, _cultureInfo)}",
				SVActualWidthValue = Math.Round(sv.ActualWidth, numberOfDigits),

				ExtentHeight = $"ExtentHeight: {sv.ExtentHeight.ToString(_dblFormat, _cultureInfo)}",
				ExtentHeightValue = Math.Round(sv.ExtentHeight, numberOfDigits),

				ExtentWidth = $"ExtentWidth: {sv.ExtentWidth.ToString(_dblFormat, _cultureInfo)}",
				ExtentWidthValue = Math.Round(sv.ExtentWidth, numberOfDigits),

				ViewportHeight = $"ViewportHeight: {sv.ViewportHeight.ToString(_dblFormat, _cultureInfo)}",
				ViewportHeightValue = Math.Round(sv.ViewportHeight, numberOfDigits),

				ViewportWidth = $"ViewportWidth: {sv.ViewportWidth.ToString(_dblFormat, _cultureInfo)}",
				ViewportWidthValue = Math.Round(sv.ViewportWidth, numberOfDigits),

				ScrollableHeight = $"ScrollableHeight: {sv.ScrollableHeight.ToString(_dblFormat, _cultureInfo)}",
				ScrollableHeightValue = Math.Round(sv.ScrollableHeight, numberOfDigits),

				ScrollableWidth = $"ScrollableWidth: {sv.ScrollableWidth.ToString(_dblFormat, _cultureInfo)}",
				ScrollableWidthValue = Math.Round(sv.ScrollableWidth, numberOfDigits),

				HorizontalOffsetValue = Math.Round(sv.HorizontalOffset, numberOfDigits),
				HorizontalOffsetAfterScrollTest = Math.Round(horizontalScrollTestResult),

				VerticalOffsetValue = Math.Round(sv.VerticalOffset, numberOfDigits),
				VerticalOffsetAfterScrollTest = Math.Round(verticalScrollTestResult)
			};
		}

		public string NeitherDisplayName => "No Padding and No Content Margin";
		public string SVPaddingDisplayName => "ScrollViewer Padding";
		public string ContentMarginDisplayName => "Content Margin";
		public string BothDisplayName => "Padding and Content Margin";

		private Dictionary<string, (string dataString, ScrollViewerContentExtentData data, Thickness scrollViewerPadding, Thickness contentMargin)> _dataForLogger = new Dictionary<string, (string dataString, ScrollViewerContentExtentData data, Thickness scrollViewerPadding, Thickness contentMargin)>();
		private void AddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(string scrollViewerName, (string dataString, ScrollViewerContentExtentData data, Thickness scrollViewerPadding, Thickness contentMargin) dataInfo, bool fromSizeChanged, [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = null, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = null, [System.Runtime.CompilerServices.CallerLineNumber] int callerLineNumber = 0)
		{
			if (string.IsNullOrEmpty(scrollViewerName))
			{
				if (callerMemberName == null)
				{
					callerMemberName = string.Empty;
				}
				if (callerFilePath == null)
				{
					callerMemberName = string.Empty;
				}
				_log.Error($"In call from '{callerMemberName}' in file '' at line number , expected argument {nameof(scrollViewerName)} to have a value but it is {(scrollViewerName == null ? "null" : "empty")}");
				return;
			}
			if (dataInfo.data == null || string.IsNullOrEmpty(dataInfo.dataString))
			{
				if (callerMemberName == null)
				{
					callerMemberName = string.Empty;
				}
				if (callerFilePath == null)
				{
					callerMemberName = string.Empty;
				}
				var errorString = $"In call from '{callerMemberName}' in file '' at line number , argument {nameof(dataInfo)} has invalid data.";
				if (dataInfo.data == null)
				{
					errorString += $" {nameof(dataInfo)}.{nameof(dataInfo.data)} is null.";
				}
				if (string.IsNullOrEmpty(dataInfo.dataString))
				{
					errorString += $" Expected {nameof(dataInfo)}.{nameof(dataInfo.dataString)} to have a value but it is {(dataInfo.dataString == null ? "null" : "empty")}.";
				}
				_log.Error(errorString);
				return;
			}
			switch (scrollViewerName)
			{
				case nameof(ContentMarginScrollViewer):
					dataInfo.data.Name = ContentMarginDisplayName;
					ScrollViewerData.RemoveAt(0);
					ScrollViewerData.Insert(0, dataInfo.data);
					break;
				case nameof(SVPaddingScrollViewer):
					dataInfo.data.Name = SVPaddingDisplayName;
					ScrollViewerData.RemoveAt(1);
					ScrollViewerData.Insert(1, dataInfo.data);
					break;
				case nameof(BothScrollViewer):
					dataInfo.data.Name = BothDisplayName;
					ScrollViewerData.RemoveAt(2);
					ScrollViewerData.Insert(2, dataInfo.data);
					break;
				case nameof(NeitherScrollViewer):
					{
						dataInfo.data.Name = NeitherDisplayName;
						// We don't want this in the collection.
						// But because it will be updated even after it's added to _dataForLogger we need to run the scroll tests here.
						var sv = NeitherScrollViewer;
						var dataToPopulate = dataInfo.data;
						RunScrollToOffsetsTests(sv, dataToPopulate);
					}
					break;
				default:
					_log.Debug($"Unexpected data with name '{scrollViewerName ?? "(null)"}");
					break;
			}
			//if (!_dataForLogger.ContainsKey(scrollViewerName) || fromSizeChanged)
			//{
			if (_dataForLogger.ContainsKey(scrollViewerName))
			{
				_dataForLogger[scrollViewerName] = dataInfo;
			}
			else
			{
				_dataForLogger.Add(scrollViewerName, dataInfo);
			}

			if (scrollViewerName == nameof(NeitherScrollViewer))
			{
				var neitherData = _dataForLogger[nameof(NeitherScrollViewer)].data;
				foreach (var item in _dataForLogger)
				{
					if (item.Key == nameof(NeitherScrollViewer))
					{
						continue;
					}
					ScrollViewer sv = null;
					switch (item.Value.data.ScrollViewerName)
					{
						case nameof(ContentMarginScrollViewer):
							sv = ContentMarginScrollViewer;
							break;
						case nameof(SVPaddingScrollViewer):
							sv = SVPaddingScrollViewer;
							break;
						case nameof(BothScrollViewer):
							sv = BothScrollViewer;
							break;
						default:
							_log.Debug($"Unknown ScrollViewer name '{item.Value.data.ScrollViewerName ?? "(null)"}'");
							break;
					}
					RunScrollToOffsetsTests(sv, item.Value.data);

					PopulateExpectedAndActualValuesForScrollViewerContentExtentData(neitherData, item.Value.data, item.Value.scrollViewerPadding, item.Value.contentMargin);
				}
			}
			else
			{
				if (_dataForLogger.ContainsKey(nameof(NeitherScrollViewer)))
				{
					ScrollViewer sv = null;
					switch (dataInfo.data.ScrollViewerName)
					{
						case nameof(ContentMarginScrollViewer):
							sv = ContentMarginScrollViewer;
							break;
						case nameof(SVPaddingScrollViewer):
							sv = SVPaddingScrollViewer;
							break;
						case nameof(BothScrollViewer):
							sv = BothScrollViewer;
							break;
						default:
							_log.Debug($"Unknown ScrollViewer name '{dataInfo.data.ScrollViewerName ?? "(null)"}'");
							break;
					}
					RunScrollToOffsetsTests(sv, dataInfo.data);

					PopulateExpectedAndActualValuesForScrollViewerContentExtentData(_dataForLogger[nameof(NeitherScrollViewer)].data, dataInfo.data, dataInfo.scrollViewerPadding, dataInfo.contentMargin);
				}
			}
			if (_dataForLogger.Count == 4)
			{
				foreach (var item in _dataForLogger)
				{
					//_log.Debug(item.Value.dataString);
				}
			}
			//}
		}

		private void RunScrollToOffsetsTests(ScrollViewer sv, ScrollViewerContentExtentData dataToPopulate)
		{
			if (dataToPopulate.HorizontalOffsetValue != 0 || dataToPopulate.VerticalOffsetValue != 0)
			{
				return;
			}
			if (sv.ChangeView(sv.ScrollableWidth, null, null, true))
			{
				if (sv.ChangeView(null, sv.ScrollableHeight, null, true))
				{
					dataToPopulate.HorizontalOffsetAfterScrollTest = sv.HorizontalOffset;
					_ = sv.ChangeView(sv.ScrollableWidth, null, null, true);
					dataToPopulate.VerticalOffsetAfterScrollTest = sv.VerticalOffset;
					if (sv.HorizontalOffset == 0 && sv.VerticalOffset == 0)
					{
						// UWP doesn't instantly update the offset values so we need to wait for it to fire the ViewChanged event for the ScrollViewer.
						return;
					}
					FinishRunScrollToOffsetsTest(sv, dataToPopulate);
				}
				else
				{
					_log.Debug($"Failed to scroll {nameof(ScrollViewer.VerticalOffset)} to {sv.ScrollableHeight} for {nameof(ScrollViewer)} {sv.Name}.");
				}
			}
			else
			{
				_log.Debug($"Failed to scroll {nameof(ScrollViewer.HorizontalOffset)} to {sv.ScrollableWidth} for {nameof(ScrollViewer)} {sv.Name}.");
			}
		}

		private void FinishRunScrollToOffsetsTest(ScrollViewer sv, ScrollViewerContentExtentData dataToPopulate)
		{
			switch (sv.Name)
			{
				case nameof(NeitherScrollViewer):
					_neitherScrollViewerScrollTestComplete = true;
					_neitherScrollViewerScrollTestHorizontalResult = sv.HorizontalOffset;
					_neitherScrollViewerScrollTestVerticalResult = sv.VerticalOffset;
					break;
				case nameof(ContentMarginScrollViewer):
					_contentMarginScrollViewerScrollTestComplete = true;
					_contentMarginScrollViewerScrollTestHorizontalResult = sv.HorizontalOffset;
					_contentMarginScrollViewerScrollTestVerticalResult = sv.VerticalOffset;
					break;
				case nameof(SVPaddingScrollViewer):
					_svPaddingScrollViewerScrollTestComplete = true;
					_svPaddingScrollViewerScrollTestHorizontalResult = sv.HorizontalOffset;
					_svPaddingScrollViewerScrollTestVerticalResult = sv.VerticalOffset;
					break;
				case nameof(BothScrollViewer):
					_bothScrollViewerScrollTestComplete = true;
					_bothScrollViewerScrollTestHorizontalResult = sv.HorizontalOffset;
					_bothScrollViewerScrollTestVerticalResult = sv.VerticalOffset;
					break;
				default:
					_log.Debug($"Unknown {nameof(ScrollViewer)} '{sv.Name ?? "(null)"}'");
					break;
			}
			dataToPopulate.HorizontalOffsetAfterScrollTest = sv.HorizontalOffset;
			dataToPopulate.VerticalOffsetAfterScrollTest = sv.VerticalOffset;
			//if (!sv.ChangeView(0, 0, null, true))
			//{
			//	_log.Debug($"Failed to scroll to 0,0 for {nameof(ScrollViewer)} {sv.Name}. Will try to scroll one axis at a time.");
			if (!sv.ChangeView(0, null, null, true))
			{
				_log.Debug($"Failed to scroll {nameof(ScrollViewer.HorizontalOffset)} to 0 for {nameof(ScrollViewer)} {sv.Name}.");
			}
			else
			{
				dataToPopulate.HorizontalOffsetValue = 0;
			}
			if (!sv.ChangeView(null, 0, null, true))
			{
				_log.Debug($"Failed to scroll {nameof(ScrollViewer.VerticalOffset)} to 0.");
			}
			else
			{
				dataToPopulate.VerticalOffsetValue = 0;
			}
			//}
			//else
			//{
			//	dataToPopulate.HorizontalOffsetValue = 0;
			//	dataToPopulate.VerticalOffsetValue = 0;
			//}
		}

		private void PopulateExpectedAndActualValuesForScrollViewerContentExtentData(ScrollViewerContentExtentData neither,
			ScrollViewerContentExtentData dataToPopulate, Thickness scrollViewerPadding, Thickness contentMargin)
		{
			int numberOfTests = 0;
			int passedTestsCount = 0;
			dataToPopulate.SVActualWidthExpectedDifference = 0;
			dataToPopulate.SVActualWidthActualDifference = neither.SVActualWidthValue - dataToPopulate.SVActualWidthValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.SVActualWidthExpectedDifference, dataToPopulate.SVActualWidthActualDifference);

			dataToPopulate.SVActualHeightExpectedDifference = 0;
			dataToPopulate.SVActualHeightActualDifference = neither.SVActualHeightValue - dataToPopulate.SVActualHeightValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.SVActualHeightExpectedDifference, dataToPopulate.SVActualHeightActualDifference);

			dataToPopulate.ViewportWidthExpectedDifference = scrollViewerPadding.Left + scrollViewerPadding.Right;
			dataToPopulate.ViewportWidthActualDifference = neither.ViewportWidthValue - dataToPopulate.ViewportWidthValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ViewportWidthExpectedDifference, dataToPopulate.ViewportWidthActualDifference);

			dataToPopulate.ViewportHeightExpectedDifference = scrollViewerPadding.Top + scrollViewerPadding.Bottom;
			dataToPopulate.ViewportHeightActualDifference = neither.ViewportHeightValue - dataToPopulate.ViewportHeightValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ViewportHeightExpectedDifference, dataToPopulate.ViewportHeightActualDifference);

			dataToPopulate.ExtentWidthExpectedDifference = -(contentMargin.Left + contentMargin.Right);
			dataToPopulate.ExtentWidthActualDifference = neither.ExtentWidthValue - dataToPopulate.ExtentWidthValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ExtentWidthExpectedDifference, dataToPopulate.ExtentWidthActualDifference);

			dataToPopulate.ExtentHeightExpectedDifference = -(contentMargin.Top + contentMargin.Bottom);
			dataToPopulate.ExtentHeightActualDifference = neither.ExtentHeightValue - dataToPopulate.ExtentHeightValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ExtentHeightExpectedDifference, dataToPopulate.ExtentHeightActualDifference);

			dataToPopulate.ContentActualWidthExpectedDifference = 0;
			dataToPopulate.ContentActualWidthActualDifference = neither.ContentActualWidthValue - dataToPopulate.ContentActualWidthValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ContentActualWidthExpectedDifference, dataToPopulate.ContentActualWidthActualDifference);

			dataToPopulate.ContentActualHeightExpectedDifference = 0;
			dataToPopulate.ContentActualHeightActualDifference = neither.ContentActualHeightValue - dataToPopulate.ContentActualHeightValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ContentActualHeightExpectedDifference, dataToPopulate.ContentActualHeightActualDifference);

			dataToPopulate.ScrollableWidthExpectedDifference = -(contentMargin.Left + contentMargin.Right + scrollViewerPadding.Left + scrollViewerPadding.Right);
			dataToPopulate.ScrollableWidthActualDifference = neither.ScrollableWidthValue - dataToPopulate.ScrollableWidthValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ScrollableWidthExpectedDifference, dataToPopulate.ScrollableWidthActualDifference);

			dataToPopulate.ScrollableHeightExpectedDifference = -(contentMargin.Top + contentMargin.Bottom + scrollViewerPadding.Top + scrollViewerPadding.Bottom);
			dataToPopulate.ScrollableHeightActualDifference = neither.ScrollableHeightValue - dataToPopulate.ScrollableHeightValue;
			numberOfTests++;
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(dataToPopulate.ScrollableHeightExpectedDifference, dataToPopulate.ScrollableHeightActualDifference);

			numberOfTests += 2;
			ScrollViewer sv = null;
			switch (dataToPopulate.ScrollViewerName)
			{
				case nameof(ContentMarginScrollViewer):
					sv = ContentMarginScrollViewer;
					break;
				case nameof(SVPaddingScrollViewer):
					sv = SVPaddingScrollViewer;
					break;
				case nameof(BothScrollViewer):
					sv = BothScrollViewer;
					break;
				default:
					_log.Debug($"Unknown ScrollViewer name '{dataToPopulate.ScrollViewerName ?? "(null)"}'");
					break;
			}
			//RunScrollToOffsetsTests(sv, dataToPopulate);
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(sv.ScrollableWidth, dataToPopulate.HorizontalOffsetAfterScrollTest);
			passedTestsCount += ScrollViewerContentExtentData.TestPassedValue(sv.ScrollableHeight, dataToPopulate.VerticalOffsetAfterScrollTest);
			dataToPopulate.UpdateTestsPassedFailedValues(numberOfTests, passedTestsCount);
		}

		private ScrollViewerContentExtentData RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(ScrollViewer sv)
		{
			var data = GetContentExtentDataForScrollViewer(sv);
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				var debugData = $"\n{sv.Name}\n{nameof(ScrollViewerPadding)}: {ScrollViewerPadding}\n{nameof(ContentMargin)}: {new Thickness()}\n{nameof(ScrollViewerBorderThickness)}: {ScrollViewerBorderThickness}\n{nameof(ContentBorderThickness)}: {ContentBorderThickness}\n{data.SVActualWidth}\n{data.ViewportWidth}\n{data.ExtentWidth}\n{data.ContentActualWidth}\n{data.ScrollableWidth}\n{data.SVActualHeight}\n{data.ViewportHeight}\n{data.ExtentHeight}\n{data.ContentActualHeight}\n{data.ScrollableHeight}\n";

				AddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv.Name, (debugData, data, sv.Padding, ((sv.Content as StackPanel)?.Margin ?? default)), true);
			}
			return data;
		}

		private void NeitherStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = NeitherScrollViewer;
			if (sv == null)
			{
				return;
			}
			NeitherScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void NeitherScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = NeitherScrollViewer;
			if (sv == null)
			{
				return;
			}
			NeitherScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void ContentMarginStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = ContentMarginScrollViewer;
			if (sv == null)
			{
				return;
			}
			ContentMarginScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void ContentMarginScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = ContentMarginScrollViewer;
			if (sv == null)
			{
				return;
			}
			ContentMarginScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void SVPaddingStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = SVPaddingScrollViewer;
			if (sv == null)
			{
				return;
			}
			SVPaddingScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void SVPaddingScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = SVPaddingScrollViewer;
			if (sv == null)
			{
				return;
			}
			SVPaddingScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void BothStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = BothScrollViewer;
			if (sv == null)
			{
				return;
			}
			BothScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void BothScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = BothScrollViewer;
			if (sv == null)
			{
				return;
			}
			BothScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
		}

		private void NeitherScrollViewer_LayoutUpdated(object sender, object e)
		{
			var sv = NeitherScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				// Avoid triggering a Windows.UI.Xaml.LayoutCycleException
				sv.LayoutUpdated -= NeitherScrollViewer_LayoutUpdated;
				NeitherScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
			}
		}

		private void ContentMarginScrollViewer_LayoutUpdated(object sender, object e)
		{
			var sv = ContentMarginScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				// Avoid triggering a Windows.UI.Xaml.LayoutCycleException
				sv.LayoutUpdated -= ContentMarginScrollViewer_LayoutUpdated;
				ContentMarginScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
			}
		}

		private void SVPaddingScrollViewer_LayoutUpdated(object sender, object e)
		{
			var sv = SVPaddingScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				// Avoid triggering a Windows.UI.Xaml.LayoutCycleException
				sv.LayoutUpdated -= SVPaddingScrollViewer_LayoutUpdated;
				SVPaddingScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
			}
		}

		private void BothScrollViewer_LayoutUpdated(object sender, object e)
		{
			var sv = BothScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				// Avoid triggering a Windows.UI.Xaml.LayoutCycleException
				sv.LayoutUpdated -= BothScrollViewer_LayoutUpdated;
				BothScrollViewerContentExtentData = RunAddDataForLoggerAndPopulateExpectedAndActualValuesForScrollViewerContentExtentData(sv);
			}
		}

		private ScrollViewerContentExtentData _neitherScrollViewerContentExtentData;
		public ScrollViewerContentExtentData NeitherScrollViewerContentExtentData
		{
			get => _neitherScrollViewerContentExtentData;
			set
			{
				if (_neitherScrollViewerContentExtentData != value)
				{
					_neitherScrollViewerContentExtentData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerContentExtentData _contentMarginScrollViewerContentExtentData;
		public ScrollViewerContentExtentData ContentMarginScrollViewerContentExtentData
		{
			get => _contentMarginScrollViewerContentExtentData;
			set
			{
				if (_contentMarginScrollViewerContentExtentData != value)
				{
					_contentMarginScrollViewerContentExtentData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerContentExtentData _svPaddingScrollViewerContentExtentData;
		public ScrollViewerContentExtentData SVPaddingScrollViewerContentExtentData
		{
			get => _svPaddingScrollViewerContentExtentData;
			set
			{
				if (_svPaddingScrollViewerContentExtentData != value)
				{
					_svPaddingScrollViewerContentExtentData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerContentExtentData _bothScrollViewerContentExtentData;
		public ScrollViewerContentExtentData BothScrollViewerContentExtentData
		{
			get => _bothScrollViewerContentExtentData;
			set
			{
				if (_bothScrollViewerContentExtentData != value)
				{
					_bothScrollViewerContentExtentData = value;
					RaisePropertyChanged();
				}
			}
		}
	}
}
