using System;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class ViewModelExtensions
{
	public static string GetViewKey(this Type viewModelType)
	{
		return viewModelType.FullName ?? viewModelType.Name;
	}
}