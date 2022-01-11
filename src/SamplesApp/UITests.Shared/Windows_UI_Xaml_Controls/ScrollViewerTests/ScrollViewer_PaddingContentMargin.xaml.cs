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
using Windows.ApplicationModel.DataTransfer;
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

			var displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
			LogicalDpi = displayInformation?.LogicalDpi ?? 0;
			RawPixelsPerViewPixel = displayInformation?.RawPixelsPerViewPixel ?? 0;
			ResolutionScale = displayInformation?.ResolutionScale ?? Windows.Graphics.Display.ResolutionScale.Invalid;

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

		private float _logicalDpi;
		public float LogicalDpi
		{
			get => _logicalDpi;
			set
			{
				var numberOfDigitsForRounding = _numberOfDigitsForRounding;
				if (_logicalDpi != (float)Math.Round(value, numberOfDigitsForRounding))
				{
					_logicalDpi = (float)Math.Round(value, numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}
		public string LogicalDpiName => nameof(LogicalDpi);

		private double _rawPixelsPerViewPixel;
		public double RawPixelsPerViewPixel
		{
			get => _rawPixelsPerViewPixel;
			set
			{
				var numberOfDigitsForRounding = _numberOfDigitsForRounding;
				if (_rawPixelsPerViewPixel != Math.Round(value, numberOfDigitsForRounding))
				{
					_rawPixelsPerViewPixel = Math.Round(value, numberOfDigitsForRounding);
					RaisePropertyChanged();
				}
			}
		}
		public string RawPixelsPerViewPixelName => nameof(RawPixelsPerViewPixel);

		private Windows.Graphics.Display.ResolutionScale _resolutionScale;
		public Windows.Graphics.Display.ResolutionScale ResolutionScale
		{
			get => _resolutionScale;
			set
			{
				if (_resolutionScale != value)
				{
					_resolutionScale = value;
					RaisePropertyChanged();
				}
			}
		}
		public string ResolutionScaleName => nameof(ResolutionScale);

		private Thickness _scrollViewerBorderThickness = new Thickness(2, 1, 3, 2);
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
		private Thickness _contentBorderThickness = new Thickness(3, 2, 4, 3);
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

		private Thickness _pagePadding;
		public Thickness PagePadding
		{
			get => _pagePadding;
			set
			{
				if (_pagePadding != value)
				{
					_pagePadding = value;
					RaisePropertyChanged();
				}
			}
		}

		private Thickness _scrollViewerMargin = new Thickness(4);
		public Thickness ScrollViewerMargin
		{
			get => _scrollViewerMargin;
			set
			{
				if (_scrollViewerMargin != value)
				{
					_scrollViewerMargin = value;
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

		private void CopyTestResults(object _, RoutedEventArgs __)
		{
			var data = new DataPackage();
			string ThicknessToString(string name, Thickness value)
			{
				return $"{name}.{nameof(value.Left)}:{value.Left}\n" +
					$"{name}.{nameof(value.Top)}:{value.Top}\n" +
					$"{name}.{nameof(value.Right)}:{value.Right}\n" +
					$"{name}.{nameof(value.Bottom)}:{value.Bottom}\n";
			}
			data.SetText($"{ThicknessToString(nameof(ScrollViewerBorderThickness), ScrollViewerBorderThickness)}{ThicknessToString(nameof(ContentBorderThickness), ContentBorderThickness)}{NeitherScrollViewerPaddingContentMarginData}\n{ContentMarginScrollViewerPaddingContentMarginData}\n{SVPaddingScrollViewerPaddingContentMarginData}\n{BothScrollViewerPaddingContentMarginData}\n");

			Clipboard.SetContent(data);
		}

		private void ForceSizeChangedUsingScrollViewerMarginAdjustment()
		{
			//var svMargin = ScrollViewerMargin;
			//var adjustment = svMargin.Left > 4 ? -2.0 : 2.0;
			//UniformThicknessAdjustment(ref svMargin, adjustment);
			//ScrollViewerMargin = svMargin;
			PagePadding = PagePadding.Left > 8 ? new Thickness(8) : new Thickness(16);
			//var svBorderThickness = ScrollViewerBorderThickness;
			//var contentBorderThickness = ContentBorderThickness;

			//UniformThicknessAdjustment(ref svBorderThickness, 1);
			//UniformThicknessAdjustment(ref contentBorderThickness, 1);
			//ScrollViewerBorderThickness = svBorderThickness;
			//ContentBorderThickness = contentBorderThickness;

			//UniformThicknessAdjustment(ref svBorderThickness, -1);
			//UniformThicknessAdjustment(ref contentBorderThickness, -1);
			////ScrollViewerBorderThickness = svBorderThickness;
			////ContentBorderThickness = contentBorderThickness;
		}

		private void UniformThicknessAdjustment(ref Thickness thickness, double adjustment)
		{
			thickness.Left += adjustment;
			thickness.Top += adjustment;
			thickness.Bottom += adjustment;
			thickness.Right += adjustment;
		}

		public List<string> ScrollBarVisibilityEnumerators => new List<string>(Enum.GetNames(typeof(ScrollBarVisibility)));

		private string _selectedVerticalScrollBarVisibilityString = Enum.GetName(typeof(ScrollBarVisibility), _verticalScrollBarVisibilityDefaultValue);
		public string SelectedVerticalScrollBarVisibilityString
		{
			get => _selectedVerticalScrollBarVisibilityString;
			set
			{
				if (value != _selectedVerticalScrollBarVisibilityString && Enum.TryParse(value, out ScrollBarVisibility visibility))
				{
					_selectedVerticalScrollBarVisibilityString = value;
					VerticalScrollBarVisibility = visibility;
					RaisePropertyChanged();
				}
			}
		}

		private const ScrollBarVisibility _verticalScrollBarVisibilityDefaultValue = ScrollBarVisibility.Disabled;
		private ScrollBarVisibility _verticalScrollBarVisibility = _verticalScrollBarVisibilityDefaultValue;
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get => _verticalScrollBarVisibility;
			set
			{
				if (_verticalScrollBarVisibility != value)
				{
					_verticalScrollBarVisibility = value;
					RaisePropertyChanged();
					NeitherScrollViewerPaddingContentMarginData.Reset();
					ContentMarginScrollViewerPaddingContentMarginData.Reset();
					SVPaddingScrollViewerPaddingContentMarginData.Reset();
					BothScrollViewerPaddingContentMarginData.Reset();
					ForceSizeChangedUsingScrollViewerMarginAdjustment();
				}
			}
		}

		private string _selectedHorizontalScrollBarVisibility = Enum.GetName(typeof(ScrollBarVisibility), _horizontalScrollBarVisibilityDefaultValue);
		public string SelectedHorizontalScrollBarVisibility
		{
			get => _selectedHorizontalScrollBarVisibility;
			set
			{
				if (_selectedHorizontalScrollBarVisibility != value && Enum.TryParse(value, out ScrollBarVisibility visibility))
				{
					_selectedHorizontalScrollBarVisibility = value;
					HorizontalScrollBarVisibility = visibility;
					RaisePropertyChanged();
				}
			}
		}
		private const ScrollBarVisibility _horizontalScrollBarVisibilityDefaultValue = ScrollBarVisibility.Disabled;
		private ScrollBarVisibility _horizontalScrollBarVisibility = _horizontalScrollBarVisibilityDefaultValue;
		public ScrollBarVisibility HorizontalScrollBarVisibility
		{
			get => _horizontalScrollBarVisibility;
			set
			{
				if (_horizontalScrollBarVisibility != value)
				{
					_horizontalScrollBarVisibility = value;
					RaisePropertyChanged();
					NeitherScrollViewerPaddingContentMarginData.Reset();
					ContentMarginScrollViewerPaddingContentMarginData.Reset();
					SVPaddingScrollViewerPaddingContentMarginData.Reset();
					BothScrollViewerPaddingContentMarginData.Reset();
					ForceSizeChangedUsingScrollViewerMarginAdjustment();
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
