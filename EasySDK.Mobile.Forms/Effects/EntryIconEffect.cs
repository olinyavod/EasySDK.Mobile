using System.Windows.Input;
using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Effects;

public class EntryIconEffect : RoutingEffect
{
	#region Properties
	
	#region AttachedProperty StartIcon

	public static readonly BindableProperty StartIconProperty = BindableProperty.CreateAttached("StartIcon", typeof(string), typeof(EntryIconEffect), default(string));

	public static void SetStartIcon(BindableObject element, string value) => element.SetValue(StartIconProperty, value);

	public static string GetStartIcon(BindableObject element) => (string) element.GetValue(StartIconProperty);

	#endregion //AttachedProperty StartIcon

	#region AttachedProperty StartIconCommand

	public static readonly BindableProperty StartIconCommandProperty = BindableProperty.CreateAttached("StartIconCommand", typeof(ICommand), typeof(EntryIconEffect), default(ICommand));

	public static void SetStartIconCommand(BindableObject element, ICommand value)
	{
		element.SetValue(StartIconCommandProperty, value);
	}

	public static ICommand GetStartIconCommand(BindableObject element)
	{
		return (ICommand) element.GetValue(StartIconCommandProperty);
	}

	#endregion //AttachedProperty StartIconCommand

	#region AttachedProperty StartIconCommandParameter

	public static readonly BindableProperty StartIconCommandParameterProperty = BindableProperty.CreateAttached(
		"StartIconCommandParameter", typeof(object), typeof(EntryIconEffect), default(object));

	public static void SetStartIconCommandParameter(BindableObject element, object value)
	{
		element.SetValue(StartIconCommandParameterProperty, value);
	}

	public static object GetStartIconCommandParameter(BindableObject element)
	{
		return (object) element.GetValue(StartIconCommandParameterProperty);
	}

	#endregion //AttachedProperty StartIconCommandParameter
	
	#region AttachedProperty Hint

	public static readonly BindableProperty HintProperty = BindableProperty.CreateAttached(
		"Hint", typeof(string), typeof(EntryIconEffect), default(string));

	public static void SetHint(BindableObject element, string value)
	{
		element.SetValue(HintProperty, value);
	}

	public static string GetHint(BindableObject element)
	{
		return (string) element.GetValue(HintProperty);
	}

	#endregion //AttachedProperty Hint

	#region AttachedProperty HintColor

	public static readonly BindableProperty HintColorProperty = BindableProperty.CreateAttached(
		"HintColor", typeof(Color?), typeof(EntryIconEffect), default(Color?));

	public static void SetHintColor(BindableObject element, Color? value)
	{
		element.SetValue(HintColorProperty, value);
	}

	public static Color? GetHintColor(BindableObject element)
	{
		return (Color?) element.GetValue(HintColorProperty);
	}

	#endregion //AttachedProperty HintColor
	
	#region AttachedProperty EndIcon

	public static readonly BindableProperty EndIconProperty = BindableProperty.CreateAttached("EndIcon", typeof(string), typeof(EntryIconEffect), default(string));

	public static void SetEndIcon(BindableObject element, string value)
	{
		element.SetValue(EndIconProperty, value);
	}

	public static string GetEndIcon(BindableObject element)
	{
		return (string) element.GetValue(EndIconProperty);
	}

	#endregion //AttachedProperty EndIcon

	#region AttachedProperty EndIconCommand

	public static readonly BindableProperty EndIconCommandProperty = BindableProperty.CreateAttached("EndIconCommand", typeof(ICommand), typeof(EntryIconEffect), default(ICommand));

	public static void SetEndIconCommand(BindableObject element, ICommand value)
	{
		element.SetValue(EndIconCommandProperty, value);
	}

	public static ICommand GetEndIconCommand(BindableObject element)
	{
		return (ICommand) element.GetValue(EndIconCommandProperty);
	}

	#endregion //AttachedProperty EndIconCommand

	#region AttachedProperty EndIconCommandParameter

	public static readonly BindableProperty EndIconCommandParameterProperty = BindableProperty.CreateAttached("EndIconCommandParameter", typeof(object), typeof(EntryIconEffect), default(object));

	public static void SetEndIconCommandParameter(BindableObject element, object value)
	{
		element.SetValue(EndIconCommandParameterProperty, value);
	}

	public static object GetEndIconCommandParameter(BindableObject element)
	{
		return (object) element.GetValue(EndIconCommandParameterProperty);
	}

	#endregion //AttachedProperty EndIconCommandParameter
	
	#region AttachedProperty IconColor

	public static readonly BindableProperty IconColorProperty = BindableProperty.CreateAttached(
		"IconColor", typeof(Color?), typeof(EntryIconEffect), default(Color?));

	public static void SetIconColor(BindableObject element, Color? value)
	{
		element.SetValue(IconColorProperty, value);
	}

	public static Color? GetIconColor(BindableObject element)
	{
		return (Color?) element.GetValue(IconColorProperty);
	}

	#endregion //AttachedProperty IconColor

	#endregion
	
	#region ctor

	public EntryIconEffect()
		: base($"{nameof(EasySDK)}.{nameof(EasySDK.Mobile)}.{nameof(EntryIconEffect)}")
	{
	}

	#endregion
}