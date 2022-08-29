﻿namespace EasySDK.Mobile.ViewModels.Themes;

public class DefaultLightPalette : ColorPaletteBase
{
	public DefaultLightPalette()
	{
		SetColor(nameof(DefaultColorThemeKeys.PrimaryColor), "#2C88D9");
		SetColor(nameof(DefaultColorThemeKeys.SecondaryColor), "#FFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.LinkColor), "#2C88D9");
		SetColor(nameof(DefaultColorThemeKeys.TextColor), "#000000");
		SetColor(nameof(DefaultColorThemeKeys.PlaceholderColor), "#A0000000");
		SetColor(nameof(DefaultColorThemeKeys.NavigationColor), "#FFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.PageBackgroundColor), "#FFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.ErrorText), "#FF0000");
		SetColor(nameof(DefaultColorThemeKeys.ErrorColor), "#FF0000");
		SetColor(nameof(DefaultColorThemeKeys.DoneColor), "#149714");
		SetColor(nameof(DefaultColorThemeKeys.DoneTextColor), "#4eb757");
		SetColor(nameof(DefaultColorThemeKeys.RefreshBackgroundColor), "#FFFFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.ItemBackgroundColor), "#FFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.SearchBackgroundColor), "#F1F1F1");
		SetColor(nameof(DefaultColorThemeKeys.SelectedBackgroundColor), "#E6E6E6");
		SetColor(nameof(DefaultColorThemeKeys.CollectionBackgroundColor), "#F1F1F1");
		SetColor(nameof(DefaultColorThemeKeys.OutgoingCallColor), "#0085FF");
		SetColor(nameof(DefaultColorThemeKeys.IncomingCallColor), "#0085FF");
		SetColor(nameof(DefaultColorThemeKeys.StartIconColor), "#FF505050");
		SetColor(nameof(DefaultColorThemeKeys.SeparatorColor), "#FF505050");
		SetColor(nameof(DefaultColorThemeKeys.FlyoutHeaderColor), "#FFFFFF");
		SetColor(nameof(DefaultColorThemeKeys.MenuIconColor), "#737373");
	}
}