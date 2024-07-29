using System.Windows.Input;
using DevExpress.XamarinForms.Editors;
using EasySDK.Mobile.ViewModels.Extensions;
using Xamarin.Forms;

namespace EasySDK.Mobile.DXPages.Behaviors
{

	public class DataFormBehavior : Behavior<EditBase>
	{
		#region Properties

		#region AttachedProperty StartIcon

		public static readonly BindableProperty StartIconProperty = BindableProperty.CreateAttached("StartIcon",
			typeof(ImageSource), typeof(DataFormBehavior), default(ImageSource));

		public static void SetStartIcon(BindableObject element, ImageSource value)
		{
			element.SetValue(StartIconProperty, value);
		}

		public static ImageSource GetStartIcon(BindableObject element)
		{
			return (ImageSource) element.GetValue(StartIconProperty);
		}

		#endregion //AttachedProperty StartIcon

		#region AttachedProperty StartIconCommand

		public static readonly BindableProperty StartIconCommandProperty = BindableProperty.CreateAttached(
			"StartIconCommand", typeof(ICommand), typeof(DataFormBehavior), default(ICommand));

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
			"StartIconCommandParameter", typeof(object), typeof(DataFormBehavior), default(object));

		public static void SetStartIconCommandParameter(BindableObject element, object value)
		{
			element.SetValue(StartIconCommandParameterProperty, value);
		}

		public static object GetStartIconCommandParameter(BindableObject element)
		{
			return element.GetValue(StartIconCommandParameterProperty);
		}

		#endregion //AttachedProperty StartIconCommandParameter

		#region AttachedProperty FocusCommand

		public static readonly BindableProperty FocusCommandProperty = BindableProperty.CreateAttached(
			"FocusCommand", typeof(ICommand), typeof(DataFormBehavior), default(ICommand));

		public static void SetFocusCommand(BindableObject element, ICommand value)
		{
			element.SetValue(FocusCommandProperty, value);
		}

		public static ICommand GetFocusCommand(BindableObject element)
		{
			return (ICommand) element.GetValue(FocusCommandProperty);
		}

		#endregion //AttachedProperty FocusCommand

		#region AttachedProperty FocusCommandParameter

		public static readonly BindableProperty FocusCommandParameterProperty = BindableProperty.CreateAttached(
			"FocusCommandParameter", typeof(object), typeof(DataFormBehavior), default(object));

		public static void SetFocusCommandParameter(BindableObject element, object value)
		{
			element.SetValue(FocusCommandParameterProperty, value);
		}

		public static object GetFocusCommandParameter(BindableObject element)
		{
			return element.GetValue(FocusCommandParameterProperty);
		}

		#endregion //AttachedProperty FocusCommandParameter

		#region AttachedProperty LostCommand

		public static readonly BindableProperty LostCommandProperty =
			BindableProperty.CreateAttached("LostCommand", typeof(ICommand), typeof(DataFormBehavior),
				default(ICommand));

		public static void SetLostCommand(BindableObject element, ICommand value)
		{
			element.SetValue(LostCommandProperty, value);
		}

		public static ICommand GetLostCommand(BindableObject element)
		{
			return (ICommand) element.GetValue(LostCommandProperty);
		}

		#endregion //AttachedProperty LostCommand

		#region AttachedProperty LostFocusCommandParameter

		public static readonly BindableProperty LostFocusCommandParameterProperty =
			BindableProperty.CreateAttached("LostFocusCommandParameter", typeof(object), typeof(DataFormBehavior),
				default(object));

		public static void SetLostFocusCommandParameter(BindableObject element, object value)
		{
			element.SetValue(LostFocusCommandParameterProperty, value);
		}

		public static object GetLostFocusCommandParameter(BindableObject element)
		{
			return element.GetValue(LostFocusCommandParameterProperty);
		}

		#endregion //AttachedProperty LostFocusCommandParameter

		#region AttachedProperty RetrunCommand

		public static readonly BindableProperty RetrunCommandProperty = BindableProperty.CreateAttached(
			"RetrunCommand", typeof(ICommand), typeof(DataFormBehavior), default(ICommand));

		public static void SetRetrunCommand(BindableObject element, ICommand value)
		{
			element.SetValue(RetrunCommandProperty, value);
		}

		public static ICommand GetRetrunCommand(BindableObject element)
		{
			return (ICommand) element.GetValue(RetrunCommandProperty);
		}

		#endregion //AttachedProperty RetrunCommand

		#region AttachedProperty EndIcon

		public static readonly BindableProperty EndIconProperty = BindableProperty.CreateAttached(
			"EndIcon", typeof(ImageSource), typeof(DataFormBehavior), default(ImageSource?));

		public static void SetEndIcon(BindableObject element, ImageSource? value)
		{
			element.SetValue(EndIconProperty, value);
		}

		public static ImageSource? GetEndIcon(BindableObject element)
		{
			return (ImageSource?) element.GetValue(EndIconProperty);
		}

		#endregion //AttachedProperty EndIcon

		#region AttachedProperty EndIconCommand

		public static readonly BindableProperty EndIconCommandProperty = BindableProperty.CreateAttached("EndIconCommand", typeof(ICommand), typeof(DataFormBehavior), default(ICommand?));

		public static void SetEndIconCommand(BindableObject element, ICommand? value)
		{
			element.SetValue(EndIconCommandProperty, value);
		}

		public static ICommand? GetEndIconCommand(BindableObject element)
		{
			return (ICommand?) element.GetValue(EndIconCommandProperty);
		}

		#endregion //AttachedProperty EndIconCommand

		#region AttachedProperty EndIconCommandParameter

		public static readonly BindableProperty EndIconCommandParameterProperty = BindableProperty.CreateAttached("EndIconCommandParameter", typeof(object), typeof(DataFormBehavior), default(object?));

		public static void SetEndIconCommandParameter(BindableObject element, object? value)
		{
			element.SetValue(EndIconCommandParameterProperty, value);
		}

		public static object? GetEndIconCommandParameter(BindableObject element)
		{
			return (object?) element.GetValue(EndIconCommandParameterProperty);
		}

		#endregion //AttachedProperty EndIconCommandParameter

		#endregion

		#region Protected methods

		protected override void OnAttachedTo(EditBase editor)
		{
			base.OnAttachedTo(editor);

			if (editor.Parent is { } parent)
			{
				if (GetStartIcon(parent) is { } startIcon)
					editor.StartIcon = startIcon;

				if (GetStartIconCommand(parent) is { } startIconCommand)
					editor.StartIconCommand = startIconCommand;

				if (GetRetrunCommand(parent) is { } returnCommand)
					editor.ReturnCommand = returnCommand;

				if (GetEndIcon(parent) is { } endIcon)
					editor.EndIcon = endIcon;

				if(GetEndIconCommand(parent) is { } endIconCommand)
					editor.EndIconCommand = endIconCommand;
			}

			editor.Focused += EditorOnFocused;
		}

		protected override void OnDetachingFrom(EditBase editor)
		{
			base.OnDetachingFrom(editor);

			editor.Focused -= EditorOnFocused;
		}

		#endregion

		#region Private methos

		private void EditorOnFocused(object sender, FocusEventArgs e)
		{
			if (sender is EditBase editor && e.IsFocused)
			{
				GetFocusCommand(editor.Parent).TryExecute(GetFocusCommandParameter(editor.Parent));
			}
		}

		#endregion
	}
}