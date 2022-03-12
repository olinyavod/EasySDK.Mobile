using Android.App;
using Android.OS;
using Android.Views;
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

	#endregion


	#region Public methods

	public static void OnThemeChanged(this Application app, Activity activity, Color lightStatusBarColor, Color darkStatusBarColor)
	{
		SetDarkStatusBarColor(app, darkStatusBarColor);
		SetLightStatusBar(app, lightStatusBarColor);
		SetActivity(app, activity);

		app.RequestedThemeChanged += FormsAppOnRequestedThemeChanged;
	}

	#endregion

	#region Private methods

	private static void FormsAppOnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
	{
		var app = (Application) sender;
		var activity = GetActivity(app);


		if (e.RequestedTheme == OSAppTheme.Dark)
		{
			ClearLightStatusBar(activity);
			var darkStatusBarColor = GetDarkStatusBarColor(app);
			activity.Window?.SetStatusBarColor(darkStatusBarColor.ToAndroid());
		}
		else
		{
			SetLightStatusBar(activity);
			var lightStatusBarColor = GetLightStatusBar(app);
			activity.Window?.SetStatusBarColor(lightStatusBarColor.ToAndroid());
		}
	}

	private static void SetLightStatusBar(Activity activity)
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

	private static void ClearLightStatusBar(Activity activity)
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
}