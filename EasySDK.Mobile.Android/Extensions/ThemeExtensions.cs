using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Xamarin.Forms.Application;

namespace EasySDK.Mobile.Android.Extensions;

public static class ThemeExtensions
{
	#region Properties

	#region AttachedProperty Activity

	public static readonly BindableProperty ActivityProperty = BindableProperty.CreateAttached(
		"Activity", typeof(Activity), typeof(ThemeExtensions), default(Activity));

	public static void SetActivity(BindableObject element, Activity value)
	{
		element.SetValue(ActivityProperty, value);
	}

	public static Activity GetActivity(BindableObject element)
	{
		return (Activity) element.GetValue(ActivityProperty);
	}

	#endregion //AttachedProperty Activity

	#region AttachedProperty LightStatusBar

	public static readonly BindableProperty LightStatusBarProperty = BindableProperty.CreateAttached(
		"LightStatusBar", typeof(Color), typeof(ThemeExtensions), default(Color));

	public static void SetLightStatusBar(BindableObject element, Color value)
	{
		element.SetValue(LightStatusBarProperty, value);
	}

	public static Color GetLightStatusBar(BindableObject element)
	{
		return (Color) element.GetValue(LightStatusBarProperty);
	}

	#endregion //AttachedProperty LightStatusBar

	#region AttachedProperty DarkStatusBarColor

	public static readonly BindableProperty DarkStatusBarColorProperty = BindableProperty.CreateAttached(
		"DarkStatusBarColor", typeof(Color), typeof(ThemeExtensions), default(Color));

	public static void SetDarkStatusBarColor(BindableObject element, Color value)
	{
		element.SetValue(DarkStatusBarColorProperty, value);
	}

	public static Color GetDarkStatusBarColor(BindableObject element)
	{
		return (Color) element.GetValue(DarkStatusBarColorProperty);
	}

	#endregion //AttachedProperty DarkStatusBarColor

	#region AttachedProperty StatusBarColorKey

	public static readonly BindableProperty StatusBarColorKeyProperty = BindableProperty.CreateAttached(
		"StatusBarColorKey", typeof(string), typeof(ThemeExtensions), default(string));

	public static void SetStatusBarColorKey(BindableObject element, string value)
	{
		element.SetValue(StatusBarColorKeyProperty, value);
	}

	public static string GetStatusBarColorKey(BindableObject element)
	{
		return (string) element.GetValue(StatusBarColorKeyProperty);
	}

	#endregion //AttachedProperty StatusBarColorKey

	#endregion


	#region Public methods

	public static void OnThemeChanged(this Application app, Activity activity, string statusColorKey)
	{
		SetStatusBarColorKey(app, statusColorKey);
		
		SetActivity(app, activity);

		FormsAppOnRequestedThemeChanged(app, new AppThemeChangedEventArgs(app.RequestedTheme));

		app.RequestedThemeChanged += FormsAppOnRequestedThemeChanged;
	}

	public static void SetNavigationBarColor(this Activity activity, Color value, bool isLight)
	{
		if (activity.Window is not { } window)
			return;

		window.SetNavigationBarColor(value.ToAndroid());

		if (isLight)
			window.DecorView.SystemUiVisibility |= (StatusBarVisibility) SystemUiFlags.LightNavigationBar;
		else
			window.DecorView.SystemUiVisibility &= ~((StatusBarVisibility) SystemUiFlags.LightNavigationBar);
	}

	public static void SetStatusBarColor(this Activity activity, Color value, bool isLight)
	{
		activity.Window?.SetStatusBarColor(value.ToAndroid());

		if (isLight)
			SetLightStatusBar(activity);
		else
			ClearLightStatusBar(activity);
	}

	public static void SetLightStatusBar(this Activity activity)
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
		{
			activity.Window?.InsetsController?.SetSystemBarsAppearance
			(
				(int)WindowInsetsControllerAppearance.LightStatusBars,
				(int)WindowInsetsControllerAppearance.LightStatusBars
			);
		}
		else
		{
			activity.Window.DecorView.SystemUiVisibility = Build.VERSION.SdkInt >= BuildVersionCodes.O
				? (StatusBarVisibility)(SystemUiFlags.LightStatusBar | SystemUiFlags.LightNavigationBar)
				: (StatusBarVisibility)SystemUiFlags.LightStatusBar;
		}
	}

	public static void ClearLightStatusBar(this Activity activity)
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.R) 
			activity.Window?.InsetsController?.SetSystemBarsAppearance
			(
				0,
				(int) WindowInsetsControllerAppearance.LightStatusBars
			);
		else if(activity.Window?.DecorView?.SystemUiVisibility is { } visibility)
		{
			var newVisibility = Build.VERSION.SdkInt switch
			{
				>= BuildVersionCodes.O => visibility & ~((StatusBarVisibility)(SystemUiFlags.LightStatusBar | SystemUiFlags.LightNavigationBar)),

				_ => visibility & ~(StatusBarVisibility)SystemUiFlags.LightStatusBar
			};

			activity.Window.DecorView.SystemUiVisibility = newVisibility;
		}
	}

	#endregion

	#region Private methods

	private static void FormsAppOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		var app = (Application) sender;
		var activity = GetActivity(app);


		if (e.RequestedTheme == OSAppTheme.Dark)
			ClearLightStatusBar(activity);
		else
			SetLightStatusBar(activity);

		var statusBarColorKey = GetStatusBarColorKey(app);
		
		var element = new BoxView();
		element.SetDynamicResource(BoxView.ColorProperty, statusBarColorKey);

		activity.Window?.SetStatusBarColor(element.Color.ToAndroid());
	}
	
	#endregion
}