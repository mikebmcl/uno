﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Input;
using FluentAssertions;
using NUnit.Framework;
using SamplesApp.UITests.TestFramework;
using Uno.Testing;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;

namespace SamplesApp.UITests.Windows_UI_Xaml_Input
{
	[TestFixture]
	public partial class NestedHandling_Tests : SampleControlUITestBase
	{
		private const string _sample = "UITests.Windows_UI_Input.PointersTests.NestedHandling";

		[Test]
		[AutoRetry]
		[InjectedPointer(PointerDeviceType.Touch)]
		public async Task When_NestedHandlesPressed_Then_ContainerStillGetsSubsequentEvents()
		{
			await RunAsync(_sample);

			var target = App.WaitForElement("_nested").Single().Rect;
			App.DragCoordinates(target.X + 10, target.Y + 10, target.Right - 10, target.Bottom - 10);

			var result = App.Marked("_result").GetDependencyPropertyValue("Text");
			result.Should().Be("Pressed SUCCESS | Released SUCCESS");
		}

		[Test]
		[AutoRetry]
		[InjectedPointer(PointerDeviceType.Touch)]
		public async Task When_Nested_Then_EnterAndExitedDoesNotBubble()
		{
			await RunAsync(_sample);

			var container = App.WaitForElement("_container").Single().Rect;
			var nested = App.WaitForElement("_nested").Single().Rect;

			App.DragCoordinates(container.X + 10, container.CenterY, container.Right - 10, nested.CenterY);

			var enterResult = App.Marked("_enterResult").GetDependencyPropertyValue("Text");
			enterResult.Should().Be("SUCCESS");

			var exitResult = App.Marked("_exitResult").GetDependencyPropertyValue("Text");
			exitResult.Should().Be("SUCCESS");
		}
	}
}
