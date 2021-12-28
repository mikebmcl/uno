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
			NeitherScrollViewerContentExtentData = new ScrollViewerContentExtentDataViewModel(NeitherDisplayName);
			ContentMarginScrollViewerContentExtentData = new ScrollViewerContentExtentDataViewModel(ContentMarginDisplayName);
			SVPaddingScrollViewerContentExtentData = new ScrollViewerContentExtentDataViewModel(SVPaddingDisplayName);
			BothScrollViewerContentExtentData = new ScrollViewerContentExtentDataViewModel(BothDisplayName);

			ScrollViewerData = new ObservableCollection<ScrollViewerContentExtentDataViewModel>(new ScrollViewerContentExtentDataViewModel[] { ContentMarginScrollViewerContentExtentData, SVPaddingScrollViewerContentExtentData, BothScrollViewerContentExtentData });

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
		private Thickness _scrollViewerPadding = new Thickness(4, 6, 11, 13);
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
		private Thickness _contentMargin = new Thickness(19, 23, 26, 32);
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

		public ObservableCollection<ScrollViewerContentExtentDataViewModel> ScrollViewerData { get; set; }

		public string NeitherDisplayName => "No Padding and No Content Margin";
		public string SVPaddingDisplayName => "ScrollViewer Padding";
		public string ContentMarginDisplayName => "Content Margin";
		public string BothDisplayName => "Padding and Content Margin";

		private ScrollViewerContentExtentDataViewModel _neitherScrollViewerContentExtentData;
		public ScrollViewerContentExtentDataViewModel NeitherScrollViewerContentExtentData
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

		private ScrollViewerContentExtentDataViewModel _contentMarginScrollViewerContentExtentData;
		public ScrollViewerContentExtentDataViewModel ContentMarginScrollViewerContentExtentData
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

		private ScrollViewerContentExtentDataViewModel _svPaddingScrollViewerContentExtentData;
		public ScrollViewerContentExtentDataViewModel SVPaddingScrollViewerContentExtentData
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

		private ScrollViewerContentExtentDataViewModel _bothScrollViewerContentExtentData;
		public ScrollViewerContentExtentDataViewModel BothScrollViewerContentExtentData
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

		private void ScrollViewer_ContentExtent_Loaded(object sender, RoutedEventArgs e)
		{
			NeitherScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
			NeitherScrollViewerContentExtentData.BeginScrollTest(NeitherScrollViewer);

			ContentMarginScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerContentExtentData.BeginScrollTest(ContentMarginScrollViewer);

			SVPaddingScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerContentExtentData.BeginScrollTest(SVPaddingScrollViewer);

			BothScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerContentExtentData.BeginScrollTest(BothScrollViewer);
		}

		private void NeitherScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = NeitherScrollViewerContentExtentData;
			var sv = NeitherScrollViewer;
			if (data != null)
			{
				NeitherScrollViewerContentExtentData.UpdateScrollTest(NeitherScrollViewer, NeitherScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void ContentMarginScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = ContentMarginScrollViewerContentExtentData;
			var sv = ContentMarginScrollViewer;
			if (data != null)
			{
				ContentMarginScrollViewerContentExtentData.UpdateScrollTest(NeitherScrollViewer, ContentMarginScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void SVPaddingScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = SVPaddingScrollViewerContentExtentData;
			var sv = SVPaddingScrollViewer;
			if (data != null)
			{
				SVPaddingScrollViewerContentExtentData.UpdateScrollTest(NeitherScrollViewer, SVPaddingScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void BothScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = BothScrollViewerContentExtentData;
			var sv = BothScrollViewer;
			if (data != null)
			{
				BothScrollViewerContentExtentData.UpdateScrollTest(NeitherScrollViewer, BothScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
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
				NeitherScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
				NeitherScrollViewerContentExtentData.BeginScrollTest(NeitherScrollViewer);
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
				ContentMarginScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
				ContentMarginScrollViewerContentExtentData.BeginScrollTest(ContentMarginScrollViewer);
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
				SVPaddingScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
				SVPaddingScrollViewerContentExtentData.BeginScrollTest(SVPaddingScrollViewer);
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
				BothScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
				BothScrollViewerContentExtentData.BeginScrollTest(BothScrollViewer);
			}
		}

		private void NeitherStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			NeitherScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
			NeitherScrollViewerContentExtentData.BeginScrollTest(NeitherScrollViewer);
		}

		private void NeitherScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			NeitherScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
			NeitherScrollViewerContentExtentData.BeginScrollTest(NeitherScrollViewer);
		}

		private void ContentMarginStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentMarginScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerContentExtentData.BeginScrollTest(ContentMarginScrollViewer);
		}

		private void ContentMarginScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentMarginScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerContentExtentData.BeginScrollTest(ContentMarginScrollViewer);
		}

		private void SVPaddingStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SVPaddingScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerContentExtentData.BeginScrollTest(SVPaddingScrollViewer);
		}

		private void SVPaddingScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SVPaddingScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerContentExtentData.BeginScrollTest(SVPaddingScrollViewer);
		}

		private void BothStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			BothScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerContentExtentData.BeginScrollTest(BothScrollViewer);
		}

		private void BothScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			BothScrollViewerContentExtentData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerContentExtentData.BeginScrollTest(BothScrollViewer);
		}
	}
}
