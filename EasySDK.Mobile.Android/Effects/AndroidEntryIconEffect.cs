using System;
using System.ComponentModel;
using Android.Content.Res;
using EasySDK.Mobile.Android.Effects;
using EasySDK.Mobile.ViewModels.Effects;
using EasySDK.Mobile.ViewModels.Extensions;
using Google.Android.Material.TextField;
using Xamarin.Forms;
using Xamarin.Forms.Material.Android;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(AndroidEntryIconEffect), nameof(EntryIconEffect))]  

namespace EasySDK.Mobile.Android.Effects;

class AndroidEntryIconEffect : PlatformEffect
{
	#region Nested classes

	class ClickListener : Java.Lang.Object, View.IOnClickListener
	{
		#region Private fields

		private readonly Action<View?> _onClick;

		#endregion

		#region ctor

		public ClickListener(Action<View?> onClick)
		{
			_onClick = onClick;
		}

		#endregion

		#region Public methods

		public void OnClick(View? v)
		{
			_onClick?.Invoke(v);
		}

		#endregion
	}

	#endregion

	#region Private fields

	private readonly ClickListener _startIconClickListener;
	private readonly ClickListener _endIconClickListener;

	#endregion

	#region ctor

	public AndroidEntryIconEffect()
	{
		_startIconClickListener = new ClickListener(v =>
		{
			var parameter = EntryIconEffect.GetStartIconCommandParameter(Element);
			var command = EntryIconEffect.GetStartIconCommand(Element);

			command?.TryExecute(parameter);
		});

		_endIconClickListener = new ClickListener(v =>
		{
			var parameter = EntryIconEffect.GetEndIconCommandParameter(Element);
			var command = EntryIconEffect.GetEndIconCommand(Element);

			command?.TryExecute(parameter);
		});
	}

	#endregion

	#region Protected methods

	protected override void OnAttached()
	{
		UpdateHint();
		UpdateHintColor();
		UpdateStartIcon();
		UpdateEndIcon();
		UpdateStartIconCommand();
		UpdateEndIconCommand();
		UpdateIconColor();
	}

	protected override void OnDetached()
	{
		if (Control is TextInputLayout layout)
		{
			layout.SetStartIconOnClickListener(null);

			layout.SetEndIconOnClickListener(null);

			_startIconClickListener.Dispose();
			_endIconClickListener.Dispose();
		}
	}

	protected override void OnElementPropertyChanged(PropertyChangedEventArgs e)
	{
		base.OnElementPropertyChanged(e);

		if (e.PropertyName == EntryIconEffect.StartIconProperty.PropertyName)
			UpdateStartIcon();
		else if(e.PropertyName == EntryIconEffect.HintColorProperty.PropertyName)
			UpdateHintColor();
		else if(e.PropertyName == EntryIconEffect.HintProperty.PropertyName)
			UpdateHint();
		else if(e.PropertyName == EntryIconEffect.EndIconProperty.PropertyName)
			UpdateEndIcon();
		else if(e.PropertyName == EntryIconEffect.StartIconCommandProperty.PropertyName)
			UpdateStartIconCommand();
		else if(e.PropertyName == EntryIconEffect.EndIconCommandProperty.PropertyName)
			UpdateEndIconCommand();
		else if(e.PropertyName == EntryIconEffect.IconColorProperty.PropertyName)
			UpdateIconColor();
	}

	#endregion

	#region Private methods

	private void UpdateIconColor()
	{
		var colorList = EntryIconEffect.GetIconColor(Element) is { } c
			? ColorStateList.ValueOf(c.ToAndroid())
			: null;

		switch (Control)
		{
			case TextInputLayout materialEditText:
				materialEditText.SetStartIconTintList(colorList);
				materialEditText.SetEndIconTintList(colorList);
				break;
		}
	}
	
	private int GetResourceId(string type, string name)
	{
		var packageName = Control.Context?.PackageName;
		return Control.Resources?.GetIdentifier(name, type, packageName) ?? 0;
	}

	private void UpdateStartIcon()
	{
		if (Control is not MaterialFormsTextInputLayoutBase text)
			return;

		if (EntryIconEffect.GetStartIcon(Element) is { } iconName
		    && !string.IsNullOrWhiteSpace(iconName))
		{
			var resId = GetResourceId("drawable", iconName);

			text.StartIconVisible = false;
			text.SetStartIconDrawable(resId);
			text.StartIconVisible = true;
		}
		else
		{
			text.StartIconVisible = false;
		}
	}

	private void UpdateEndIcon()
	{
		if (Control is not TextInputLayout text)
			return;

		if (EntryIconEffect.GetEndIcon(Element) is { } iconName
		    && !string.IsNullOrWhiteSpace(iconName))
		{
			var resId = GetResourceId("drawable", iconName);

			text.EndIconVisible = true;
			text.SetEndIconDrawable(resId);
		}
		else
		{
			text.EndIconVisible = false;
		}
	}

	private void UpdateHintColor()
	{
		if (EntryIconEffect.GetHintColor(Element) is not { } color)
			return;

		var textColor = Element switch
		{
			Entry entry => entry.TextColor,
			DatePicker datePicker => datePicker.TextColor,
			TimePicker timePicker => timePicker.TextColor,
			Editor editor => editor.TextColor,
			Picker picker => picker.TextColor,

			_ => Color.Default
		};

		switch (Control)
		{
			case MaterialFormsTextInputLayoutBase layout:
				layout.ApplyTheme(textColor, color);
				break;
		}
	}

	private void UpdateHint()
	{
		if (EntryIconEffect.GetHint(Element) is not { } hint)
			return;
		
		switch (Control)
		{
			case MaterialFormsTextInputLayoutBase layout:
				layout.SetHint(hint,(VisualElement) Element);
				break;
		}
	}

	private void UpdateStartIconCommand()
	{
		var command = EntryIconEffect.GetStartIconCommand(Element);

		switch (Control)
		{
			case TextInputLayout layout:
				layout.SetStartIconOnClickListener(command != null ? _startIconClickListener : null);
				break;
		}
	}

	private void UpdateEndIconCommand()
	{
		var command = EntryIconEffect.GetEndIconCommand(Element);

		switch (Control)
		{
			case TextInputLayout layout:
				layout.SetEndIconOnClickListener(command != null ? _endIconClickListener : null);
				break;
		}
	}

	#endregion
}