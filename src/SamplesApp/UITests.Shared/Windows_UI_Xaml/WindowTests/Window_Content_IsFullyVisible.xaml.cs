using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Uno.UI.Samples.Controls;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UITests.Windows_UI_Xaml.WindowTests
{
	[Sample("Window")]
	public sealed partial class Window_Content_IsFullyVisible : UserControl
    {
        public Window_Content_IsFullyVisible()
        {
            this.InitializeComponent();
			Window_Content_IsFullyVisible_Content.SizeChanged += Window_Content_IsFullyVisible_Content_SizeChanged;
			SetDiagnosticInfoTextBlocksText();
		}

		private void SetDiagnosticInfoTextBlocksText()
		{
			// Diagnostic information that's helpful when this test fails and you want to examine a screenshot to compare the size of the content versus its actual size.
			DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
			ScaledActualWidthTextBlock.Text = $"Scaled {nameof(Window_Content_IsFullyVisible_Content.ActualWidth)} = {Window_Content_IsFullyVisible_Content.ActualWidth * displayInformation.RawPixelsPerViewPixel}";
			ScaledActualHeightTextBlock.Text = $"Scaled {nameof(Window_Content_IsFullyVisible_Content.ActualHeight)} = {Window_Content_IsFullyVisible_Content.ActualHeight * displayInformation.RawPixelsPerViewPixel}";
			LogicalDpiTextBlock.Text = $"{nameof(DisplayInformation.LogicalDpi)} = {displayInformation.LogicalDpi}";
			RawPixelsPerViewPixelTextBlock.Text = $"{nameof(DisplayInformation.RawPixelsPerViewPixel)} = {displayInformation.RawPixelsPerViewPixel}";
			ResolutionScaleTextBlock.Text = $"{nameof(DisplayInformation.ResolutionScale)} = {displayInformation.ResolutionScale}";
			ActualWidthTextBlock.Text = $"{nameof(Window_Content_IsFullyVisible_Content.ActualWidth)} = {Window_Content_IsFullyVisible_Content.ActualWidth}";
			ActualHeightTextBlock.Text = $"{nameof(Window_Content_IsFullyVisible_Content.ActualHeight)} = {Window_Content_IsFullyVisible_Content.ActualHeight}";
		}

		private void Window_Content_IsFullyVisible_Content_SizeChanged(object sender, SizeChangedEventArgs args)
		{
			SetDiagnosticInfoTextBlocksText();
		}
	}
}
