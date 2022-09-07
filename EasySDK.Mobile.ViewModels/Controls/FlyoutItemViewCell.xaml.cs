using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.ViewModels.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FlyoutItemViewCell
	{
		#region Properties

		#region AttachedProperty AdditionalContent

		public static readonly BindableProperty AdditionalContentProperty = BindableProperty.CreateAttached("AdditionalContent", typeof(View), typeof(FlyoutItemViewCell), default(View));

		public static void SetAdditionalContent(BindableObject element, View value)
		{
			element.SetValue(AdditionalContentProperty, value);
		}

		public static View GetAdditionalContent(BindableObject element)
		{
			return (View) element.GetValue(AdditionalContentProperty);
		}

		#endregion //AttachedProperty AdditionalContent

		#endregion

		public FlyoutItemViewCell()
		{
			InitializeComponent();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext is FlyoutItem item)
			{
				PART_AdditionalContent.Content = GetAdditionalContent(item);
			}
		}
	}
}