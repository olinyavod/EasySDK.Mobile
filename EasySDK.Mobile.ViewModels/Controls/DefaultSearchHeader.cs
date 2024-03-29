﻿using EasySDK.Mobile.ViewModels.Themes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Controls
{
	public class DefaultSearchHeader : SearchHandler
	{
		public DefaultSearchHeader()
		{
			Placeholder = Properties.Resources.SearchDots;
			SearchBoxVisibility = SearchBoxVisibility.Collapsible;

			var application = Application.Current;
			application.RequestedThemeChanged += CurrentOnRequestedThemeChanged;

			CurrentOnRequestedThemeChanged(application, new AppThemeChangedEventArgs(application.UserAppTheme));
		}

		private void CurrentOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
		{
			var app = (Application)sender;

			var resources = app.Resources;

			if (resources.TryGetValue(nameof(DefaultColorThemeKeys.PlaceholderColor), out var placeHolderColor))
				SetValue(PlaceholderColorProperty, placeHolderColor);

			if (resources.TryGetValue(nameof(DefaultColorThemeKeys.SearchBackgroundColor), out var backgroundColor))
				SetValue(BackgroundColorProperty, backgroundColor);

			if (resources.TryGetValue(nameof(DefaultColorThemeKeys.TextColor), out var textColor))
				SetValue(TextColorProperty, textColor);
		}
	}
}
