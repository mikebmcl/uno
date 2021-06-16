﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Windows.UI.Xaml.Controls
{
	partial class CommandBar
	{
		public Style CommandBarOverflowPresenterStyle
		{
			get { return (Style)this.GetValue(CommandBarOverflowPresenterStyleProperty); }
			set { this.SetValue(CommandBarOverflowPresenterStyleProperty, value); }
		}
		public static DependencyProperty CommandBarOverflowPresenterStyleProperty { get; } =
			DependencyProperty.Register(nameof(CommandBarOverflowPresenterStyle), typeof(Style), typeof(CommandBar), new PropertyMetadata(null));

		public CommandBarTemplateSettings CommandBarTemplateSettings
		{
			get { return (CommandBarTemplateSettings)this.GetValue(CommandBarTemplateSettingsProperty); }
			set { this.SetValue(CommandBarTemplateSettingsProperty, value); }
		}
		public static DependencyProperty CommandBarTemplateSettingsProperty { get; } =
			DependencyProperty.Register(nameof(CommandBarTemplateSettings), typeof(CommandBarTemplateSettings), typeof(CommandBar), new PropertyMetadata(null));

		public CommandBarDefaultLabelPosition CommandBarDefaultLabelPosition
		{
			get { return (CommandBarDefaultLabelPosition)this.GetValue(CommandBarDefaultLabelPositionProperty); }
			set { this.SetValue(CommandBarDefaultLabelPositionProperty, value); }
		}
		public static DependencyProperty CommandBarDefaultLabelPositionProperty { get; } =
			DependencyProperty.Register(nameof(CommandBarDefaultLabelPosition), typeof(CommandBarDefaultLabelPosition), typeof(CommandBar), new PropertyMetadata(CommandBarDefaultLabelPosition.Bottom));

		public bool IsDynamicOverflowEnabled
		{
			get { return (bool)this.GetValue(IsDynamicOverflowEnabledProperty); }
			set { this.SetValue(IsDynamicOverflowEnabledProperty, value); }
		}
		public static DependencyProperty IsDynamicOverflowEnabledProperty { get; } =
			DependencyProperty.Register(nameof(IsDynamicOverflowEnabled), typeof(bool), typeof(CommandBar), new PropertyMetadata(true));

		public CommandBarOverflowButtonVisibility CommandBarOverflowButtonVisibility
		{
			get { return (CommandBarOverflowButtonVisibility)this.GetValue(CommandBarOverflowButtonVisibilityProperty); }
			set { this.SetValue(CommandBarOverflowButtonVisibilityProperty, value); }
		}
		public static DependencyProperty CommandBarOverflowButtonVisibilityProperty { get; } =
			DependencyProperty.Register(nameof(CommandBarOverflowButtonVisibility), typeof(CommandBarOverflowButtonVisibility), typeof(CommandBar), new PropertyMetadata(CommandBarOverflowButtonVisibility.Auto));

		public IObservableVector<ICommandBarElement> PrimaryCommands
		{
			get { return (IObservableVector<ICommandBarElement>)this.GetValue(PrimaryCommandsProperty); }
			set { this.SetValue(PrimaryCommandsProperty, value); }
		}
		public static DependencyProperty PrimaryCommandsProperty { get; } =
			DependencyProperty.Register(nameof(PrimaryCommands), typeof(IObservableVector<ICommandBarElement>), typeof(CommandBar), new FrameworkPropertyMetadata(default(IObservableVector<ICommandBarElement>), FrameworkPropertyMetadataOptions.ValueInheritsDataContext));

		public IObservableVector<ICommandBarElement> SecondaryCommands
		{
			get { return (IObservableVector<ICommandBarElement>)this.GetValue(SecondaryCommandsProperty); }
			set { this.SetValue(SecondaryCommandsProperty, value); }
		}

		public static DependencyProperty SecondaryCommandsProperty { get; } =
			DependencyProperty.Register(nameof(SecondaryCommands), typeof(IObservableVector<ICommandBarElement>), typeof(CommandBar), new FrameworkPropertyMetadata(default(IObservableVector<ICommandBarElement>), FrameworkPropertyMetadataOptions.ValueInheritsDataContext));
	}
}