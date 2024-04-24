using Xamarin.Forms;

namespace EasySDK.Mobile.Forms.Behaviors;

public class ResourceDictionaryBehavior : Behavior<VisualElement>
{
	#region Propeties

	#region AttachedProperty Resources

	public static readonly BindableProperty ResourcesProperty = BindableProperty.CreateAttached(
		"Resources", typeof(ResourceDictionary), typeof(ResourceDictionaryBehavior), default(ResourceDictionary));

	public static void SetResources(BindableObject element, ResourceDictionary value)
	{
		element.SetValue(ResourcesProperty, value);
	}

	public static ResourceDictionary GetResources(BindableObject element)
	{
		return (ResourceDictionary) element.GetValue(ResourcesProperty);
	}

	#endregion //AttachedProperty Resources
	
	#endregion

	protected override void OnAttachedTo(VisualElement bindable)
	{
		base.OnAttachedTo(bindable);

		if (GetResources(bindable) is { } resource)
			bindable.Resources.MergedDictionaries.Add(resource);
	}
}