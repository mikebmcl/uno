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
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Sample("ScrollViewer", Description = "Checks ScrollViewer Margin and Content Padding layout.")]
	public sealed partial class ScrollViewer_PaddingContentMargin : Page, INotifyPropertyChanged
	{
		public ScrollViewer_PaddingContentMargin()
		{
			this.InitializeComponent();

			// Create with three empty entries because we want the data order to match the order of the ScrollViewers
			NeitherScrollViewerPaddingContentMarginData = new ScrollViewerPaddingContentMarginViewModel(NeitherDisplayName);
			ContentMarginScrollViewerPaddingContentMarginData = new ScrollViewerPaddingContentMarginViewModel(ContentMarginDisplayName);
			SVPaddingScrollViewerPaddingContentMarginData = new ScrollViewerPaddingContentMarginViewModel(SVPaddingDisplayName);
			BothScrollViewerPaddingContentMarginData = new ScrollViewerPaddingContentMarginViewModel(BothDisplayName);

			ScrollViewerData = new ObservableCollection<ScrollViewerPaddingContentMarginViewModel>(new ScrollViewerPaddingContentMarginViewModel[] { ContentMarginScrollViewerPaddingContentMarginData, SVPaddingScrollViewerPaddingContentMarginData, BothScrollViewerPaddingContentMarginData });

			SizeChanged += ScrollViewer_PaddingContentMargin_SizeChanged;
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
			Loaded += ScrollViewer_PaddingContentMargin_Loaded;
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

		public ObservableCollection<ScrollViewerPaddingContentMarginViewModel> ScrollViewerData { get; set; }

		public string NeitherDisplayName => "No Padding and No Content Margin";
		public string SVPaddingDisplayName => "ScrollViewer Padding";
		public string ContentMarginDisplayName => "Content Margin";
		public string BothDisplayName => "Padding and Content Margin";

		private ScrollViewerPaddingContentMarginViewModel _neitherScrollViewerPaddingContentMarginData;
		public ScrollViewerPaddingContentMarginViewModel NeitherScrollViewerPaddingContentMarginData
		{
			get => _neitherScrollViewerPaddingContentMarginData;
			set
			{
				if (_neitherScrollViewerPaddingContentMarginData != value)
				{
					_neitherScrollViewerPaddingContentMarginData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerPaddingContentMarginViewModel _contentMarginScrollViewerPaddingContentMarginData;
		public ScrollViewerPaddingContentMarginViewModel ContentMarginScrollViewerPaddingContentMarginData
		{
			get => _contentMarginScrollViewerPaddingContentMarginData;
			set
			{
				if (_contentMarginScrollViewerPaddingContentMarginData != value)
				{
					_contentMarginScrollViewerPaddingContentMarginData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerPaddingContentMarginViewModel _svPaddingScrollViewerPaddingContentMarginData;
		public ScrollViewerPaddingContentMarginViewModel SVPaddingScrollViewerPaddingContentMarginData
		{
			get => _svPaddingScrollViewerPaddingContentMarginData;
			set
			{
				if (_svPaddingScrollViewerPaddingContentMarginData != value)
				{
					_svPaddingScrollViewerPaddingContentMarginData = value;
					RaisePropertyChanged();
				}
			}
		}

		private ScrollViewerPaddingContentMarginViewModel _bothScrollViewerPaddingContentMarginData;
		public ScrollViewerPaddingContentMarginViewModel BothScrollViewerPaddingContentMarginData
		{
			get => _bothScrollViewerPaddingContentMarginData;
			set
			{
				if (_bothScrollViewerPaddingContentMarginData != value)
				{
					_bothScrollViewerPaddingContentMarginData = value;
					RaisePropertyChanged();
				}
			}
		}

		private void ScrollViewer_PaddingContentMargin_Loaded(object sender, RoutedEventArgs e)
		{
			NeitherScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
			NeitherScrollViewerPaddingContentMarginData.BeginScrollTest(NeitherScrollViewer);

			ContentMarginScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerPaddingContentMarginData.BeginScrollTest(ContentMarginScrollViewer);

			SVPaddingScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerPaddingContentMarginData.BeginScrollTest(SVPaddingScrollViewer);

			BothScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerPaddingContentMarginData.BeginScrollTest(BothScrollViewer);
		}

		private void NeitherScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = NeitherScrollViewerPaddingContentMarginData;
			var sv = NeitherScrollViewer;
			if (data != null)
			{
				NeitherScrollViewerPaddingContentMarginData.UpdateScrollTest(NeitherScrollViewer, NeitherScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void ContentMarginScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = ContentMarginScrollViewerPaddingContentMarginData;
			var sv = ContentMarginScrollViewer;
			if (data != null)
			{
				ContentMarginScrollViewerPaddingContentMarginData.UpdateScrollTest(NeitherScrollViewer, ContentMarginScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void SVPaddingScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = SVPaddingScrollViewerPaddingContentMarginData;
			var sv = SVPaddingScrollViewer;
			if (data != null)
			{
				SVPaddingScrollViewerPaddingContentMarginData.UpdateScrollTest(NeitherScrollViewer, SVPaddingScrollViewer);
				data.HorizontalOffsetValue = sv.HorizontalOffset;
				data.VerticalOffsetValue = sv.VerticalOffset;
			}
		}

		private void BothScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			var data = BothScrollViewerPaddingContentMarginData;
			var sv = BothScrollViewer;
			if (data != null)
			{
				BothScrollViewerPaddingContentMarginData.UpdateScrollTest(NeitherScrollViewer, BothScrollViewer);
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
				NeitherScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
				NeitherScrollViewerPaddingContentMarginData.BeginScrollTest(NeitherScrollViewer);
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
				ContentMarginScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
				ContentMarginScrollViewerPaddingContentMarginData.BeginScrollTest(ContentMarginScrollViewer);
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
				SVPaddingScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
				SVPaddingScrollViewerPaddingContentMarginData.BeginScrollTest(SVPaddingScrollViewer);
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
				BothScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
				BothScrollViewerPaddingContentMarginData.BeginScrollTest(BothScrollViewer);
			}
		}

		private void UpdateForNeither()
		{
			NeitherScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, NeitherScrollViewer);
			NeitherScrollViewerPaddingContentMarginData.BeginScrollTest(NeitherScrollViewer);

			ContentMarginScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerPaddingContentMarginData.BeginScrollTest(ContentMarginScrollViewer);

			SVPaddingScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerPaddingContentMarginData.BeginScrollTest(SVPaddingScrollViewer);

			BothScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerPaddingContentMarginData.BeginScrollTest(BothScrollViewer);
		}

		private void ScrollViewer_PaddingContentMargin_SizeChanged(object sender, SizeChangedEventArgs args)
		{
			NeitherScrollViewer.LayoutUpdated += NeitherScrollViewer_LayoutUpdated;
			ContentMarginScrollViewer.LayoutUpdated += ContentMarginScrollViewer_LayoutUpdated;
			SVPaddingScrollViewer.LayoutUpdated += SVPaddingScrollViewer_LayoutUpdated;
			BothScrollViewer.LayoutUpdated += BothScrollViewer_LayoutUpdated;
			UpdateForNeither();
		}

		private void NeitherStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateForNeither();
		}

		private void NeitherScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateForNeither();
		}

		private void ContentMarginStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentMarginScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerPaddingContentMarginData.BeginScrollTest(ContentMarginScrollViewer);
		}

		private void ContentMarginScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ContentMarginScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, ContentMarginScrollViewer);
			ContentMarginScrollViewerPaddingContentMarginData.BeginScrollTest(ContentMarginScrollViewer);
		}

		private void SVPaddingStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SVPaddingScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerPaddingContentMarginData.BeginScrollTest(SVPaddingScrollViewer);
		}

		private void SVPaddingScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			SVPaddingScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, SVPaddingScrollViewer);
			SVPaddingScrollViewerPaddingContentMarginData.BeginScrollTest(SVPaddingScrollViewer);
		}

		private void BothStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			BothScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerPaddingContentMarginData.BeginScrollTest(BothScrollViewer);
		}

		private void BothScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			BothScrollViewerPaddingContentMarginData.UpdateTestsPassedFailedValues(NeitherScrollViewer, BothScrollViewer);
			BothScrollViewerPaddingContentMarginData.BeginScrollTest(BothScrollViewer);
		}
	}
}
