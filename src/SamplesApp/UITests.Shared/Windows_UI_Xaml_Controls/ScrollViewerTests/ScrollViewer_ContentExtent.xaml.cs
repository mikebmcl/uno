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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UITests.Windows_UI_Xaml_Controls.ScrollViewerTests
{
	public class ScrollViewerContentExtentData
	{
		public string SVActualWidth { get; set; }
		public string SVActualHeight { get; set; }
		public string ViewPortWidth { get; set; }
		public string ViewPortHeight { get; set; }
		public string ExtentWidth { get; set; }
		public string ExtentHeight { get; set; }
		public string ContentActualWidth { get; set; }
		public string ContentActualHeight { get; set; }
		public string ScrollableWidth { get; set; }
		public string ScrollableHeight { get; set; }
	}

	[SampleControlInfo(category: "ScrollViewer")]
	public sealed partial class ScrollViewer_ContentExtent : Page, INotifyPropertyChanged
	{
		public ScrollViewer_ContentExtent()
		{
			this.InitializeComponent();

			NeitherStackPanel.SizeChanged += NeitherStackPanel_SizeChanged;
			NeitherScrollViewer.SizeChanged += NeitherScrollViewer_SizeChanged;
			ContentMarginStackPanel.SizeChanged += ContentMarginStackPanel_SizeChanged;
			ContentMarginScrollViewer.SizeChanged += ContentMarginScrollViewer_SizeChanged;
			SVPaddingStackPanel.SizeChanged += SVPaddingStackPanel_SizeChanged;
			SVPaddingScrollViewer.SizeChanged += SVPaddingScrollViewer_SizeChanged;
			BothStackPanel.SizeChanged += BothStackPanel_SizeChanged;
			BothScrollViewer.SizeChanged += BothScrollViewer_SizeChanged;
			// Note: We need to use LayoutUpdated events because the ViewPort* and Extent* data isn't ready (except in UWP) when SizeChanged fires for the first time and we don't want to have to resize the window just to get the data.
			NeitherScrollViewer.LayoutUpdated += NeitherScrollViewer_LayoutUpdated;
			ContentMarginScrollViewer.LayoutUpdated += ContentMarginScrollViewer_LayoutUpdated;
			SVPaddingScrollViewer.LayoutUpdated += SVPaddingScrollViewer_LayoutUpdated;
			BothScrollViewer.LayoutUpdated += BothScrollViewer_LayoutUpdated;
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		string _dblFormat = "F";
		System.Globalization.CultureInfo _cultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
		private ScrollViewerContentExtentData GetContentExtentDataForScrollViewer(ScrollViewer sv) => new ScrollViewerContentExtentData() { ContentActualHeight = $"Content ActualHeight: {((sv.Content as FrameworkElement)?.ActualHeight ?? double.NaN).ToString(_dblFormat, _cultureInfo)}", ContentActualWidth = $"Content ActualWidth: {((sv.Content as FrameworkElement)?.ActualWidth ?? double.NaN).ToString(_dblFormat, _cultureInfo)}", SVActualHeight = $"ScrollViewer ActualHeight: {sv.ActualHeight.ToString(_dblFormat, _cultureInfo)}", SVActualWidth = $"ScrollViewer ActualWidth: {sv.ActualWidth.ToString(_dblFormat, _cultureInfo)}", ExtentHeight = $"ExtentHeight: {sv.ExtentHeight.ToString(_dblFormat, _cultureInfo)}", ExtentWidth = $"ExtentWidth: {sv.ExtentWidth.ToString(_dblFormat, _cultureInfo)}", ViewPortHeight = $"ViewportHeight: {sv.ViewportHeight.ToString(_dblFormat, _cultureInfo)}", ViewPortWidth = $"ViewportWidth: {sv.ViewportWidth.ToString(_dblFormat, _cultureInfo)}", ScrollableHeight = $"ScrollableHeight: {sv.ScrollableHeight.ToString(_dblFormat, _cultureInfo)}", ScrollableWidth = $"ScrollableWidth: {sv.ScrollableWidth.ToString(_dblFormat, _cultureInfo)}" };

		private void NeitherStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = NeitherScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= NeitherScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			NeitherScrollViewerContentExtentData = data;
		}

		private void NeitherScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = NeitherScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= NeitherScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			NeitherScrollViewerContentExtentData = data;
		}

		private void ContentMarginStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = ContentMarginScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= ContentMarginScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			ContentMarginScrollVieweContentExtentData = data;
		}

		private void ContentMarginScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = ContentMarginScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= ContentMarginScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			ContentMarginScrollVieweContentExtentData = data;
		}

		private void SVPaddingStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = SVPaddingScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= SVPaddingScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			SVPaddingScrollViewerContentExtentData = data;
		}

		private void SVPaddingScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = SVPaddingScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= SVPaddingScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			SVPaddingScrollViewerContentExtentData = data;
		}

		private void BothStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = BothScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= BothScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			BothScrollViewerContentExtentData = data;
		}

		private void BothScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var sv = BothScrollViewer;
			if (sv == null)
			{
				return;
			}
			if (sv.ExtentWidth != 0 && sv.ExtentHeight != 0 && sv.ViewportWidth != 0 && sv.ViewportHeight != 0)
			{
				sv.LayoutUpdated -= BothScrollViewer_LayoutUpdated;
			}
			var data = GetContentExtentDataForScrollViewer(sv);
			BothScrollViewerContentExtentData = data;
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
				var data = GetContentExtentDataForScrollViewer(sv);
				NeitherScrollViewerContentExtentData = data;
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
				var data = GetContentExtentDataForScrollViewer(sv);
				ContentMarginScrollVieweContentExtentData = data;
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
				var data = GetContentExtentDataForScrollViewer(sv);
				SVPaddingScrollViewerContentExtentData = data;
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
				var data = GetContentExtentDataForScrollViewer(sv);
				BothScrollViewerContentExtentData = data;
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

		private ScrollViewerContentExtentData _contentMarginScrollVieweContentExtentData;
		public ScrollViewerContentExtentData ContentMarginScrollVieweContentExtentData
		{
			get => _contentMarginScrollVieweContentExtentData;
			set
			{
				if (_contentMarginScrollVieweContentExtentData != value)
				{
					_contentMarginScrollVieweContentExtentData = value;
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
