using Xamarin.Forms;

namespace EasySDK.Mobile.ViewModels.Effects;

public class ShowLinesEffect : RoutingEffect
{
	#region Properties

	#region AttachedProperty MinLines

	public static readonly BindableProperty MinLinesProperty = BindableProperty.CreateAttached("MinLines", typeof(int?), typeof(ShowLinesEffect), default(int?));

	public static void SetMinLines(BindableObject element, int? value)
	{
		element.SetValue(MinLinesProperty, value);
	}

	public static int? GetMinLines(BindableObject element)
	{
		return (int?) element.GetValue(MinLinesProperty);
	}

	#endregion //AttachedProperty MinLines

	#region AttachedProperty MaxLines

	public static readonly BindableProperty MaxLinesProperty = BindableProperty.CreateAttached(
		"MaxLines", typeof(int?), typeof(ShowLinesEffect), default(int?));

	public static void SetMaxLines(BindableObject element, int? value)
	{
		element.SetValue(MaxLinesProperty, value);
	}

	public static int? GetMaxLines(BindableObject element)
	{
		return (int?) element.GetValue(MaxLinesProperty);
	}

	#endregion //AttachedProperty MaxLines

	#region AttachedProperty VerticalOptions

	public static readonly BindableProperty FillVerticalProperty = BindableProperty.CreateAttached("FillVertical", typeof(bool), typeof(ShowLinesEffect), default(bool));

	public static void SetFillVertical(BindableObject element, bool value)
	{
		element.SetValue(FillVerticalProperty, value);
	}

	public static bool GetFillVertical(BindableObject element)
	{
		return (bool) element.GetValue(FillVerticalProperty);
	}

	#endregion //AttachedProperty VerticalOptions

	#endregion

	#region ctor

	public ShowLinesEffect()
		: base($"{nameof(EasySDK)}.{nameof(Mobile)}.{nameof(ShowLinesEffect)}")
	{
	}

	#endregion
}