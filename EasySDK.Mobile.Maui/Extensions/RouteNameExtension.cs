using EasySDK.Mobile.ViewModels.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace EasySDK.Mobile.Maui.Extensions;

[ContentProperty(nameof(ViewModelType))]
public class RouteNameExtension : IMarkupExtension
{
	public Type? ViewModelType { get; set; }
	
	public object ProvideValue(IServiceProvider serviceProvider)
	{
		return ViewModelType?.GetViewKey() ?? string.Empty;
	}
}