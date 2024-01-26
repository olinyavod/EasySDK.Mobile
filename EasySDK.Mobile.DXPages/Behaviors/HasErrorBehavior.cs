using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.XamarinForms.Editors;
using Xamarin.Forms;

namespace EasySDK.Mobile.DXPages.Behaviors
{
	public class HasErrorBehavior : Behavior<EditBase>
	{
		#region Nested classes

		class ErrorsChangedController : IDisposable
		{
			private readonly EditBase _editor;

			private INotifyDataErrorInfo? _dataErrorInfo;

			public ErrorsChangedController(EditBase editor)
			{
				_editor = editor;

				_editor.BindingContextChanged += EditorOnBindingContextChanged;

				InitErrorsHandlers(editor);
			}

			private void DataErrorInfoOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
			{
				if (GetFieldName(_editor) != e.PropertyName)
					return;

				_editor.HasError = _dataErrorInfo?.GetErrors(e.PropertyName)?.OfType<object>().Any() ?? false;
			}

			private void InitErrorsHandlers(EditBase editor)
			{
				if (editor.BindingContext is INotifyDataErrorInfo dataErrorInfo)
				{
					_dataErrorInfo               =  dataErrorInfo;
					_dataErrorInfo.ErrorsChanged += DataErrorInfoOnErrorsChanged;
				}
			}

			private void EditorOnBindingContextChanged(object sender, EventArgs e)
			{
				if (_dataErrorInfo != null)
					_dataErrorInfo.ErrorsChanged -= DataErrorInfoOnErrorsChanged;

				InitErrorsHandlers(_editor);
			}

			public void Dispose()
			{
				if (_dataErrorInfo != null)
					_dataErrorInfo.ErrorsChanged -= DataErrorInfoOnErrorsChanged;
				_editor.BindingContextChanged -= EditorOnBindingContextChanged;
			}
		}

		#endregion

		#region AttachedProperty FieldName

		public static readonly BindableProperty FieldNameProperty =
			BindableProperty.CreateAttached("FieldName", typeof(string), typeof(HasErrorBehavior), default(string));

		public static void SetFieldName(BindableObject element, string? value)
		{
			element.SetValue(FieldNameProperty, value);
		}

		public static string? GetFieldName(BindableObject element)
		{
			return (string?) element.GetValue(FieldNameProperty);
		}

		#endregion //AttachedProperty FieldName

		#region AttachedProperty ErrorsController

		private static readonly BindableProperty ErrorsControllerProperty = BindableProperty.CreateAttached(
			"ErrorsController", typeof(ErrorsChangedController), typeof(HasErrorBehavior),
			default(ErrorsChangedController));

		private static void SetErrorsController(BindableObject element, ErrorsChangedController value)
		{
			element.SetValue(ErrorsControllerProperty, value);
		}

		private static ErrorsChangedController GetErrorsController(BindableObject element)
		{
			return (ErrorsChangedController) element.GetValue(ErrorsControllerProperty);
		}

		#endregion //AttachedProperty ErrorsController

		protected override void OnAttachedTo(EditBase bindable)
		{
			base.OnAttachedTo(bindable);

			var controller = new ErrorsChangedController(bindable);

			SetErrorsController(bindable, controller);
		}

		protected override void OnDetachingFrom(EditBase bindable)
		{
			base.OnDetachingFrom(bindable);

			if (GetErrorsController(bindable) is { } controller)
			{
				controller.Dispose();
			}
		}
	}
}