using System;
using System.Globalization;
using System.Linq;
using EasySDK.Mobile.ViewModels.Managers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasySDK.Mobile.DXPages.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PhotosGalleryControl
	{
		#region Properties

		#region DependencyProperty PhotoWidth

		public static readonly BindableProperty PhotoWidthProperty = BindableProperty.Create(nameof(PhotoWidth),
			typeof(FlexBasis), typeof(PhotosGalleryControl), default(FlexBasis));

		public FlexBasis PhotoWidth
		{
			get => (FlexBasis) GetValue(PhotoWidthProperty);
			set => SetValue(PhotoWidthProperty, value);
		}

		#endregion // DependencyProperty PhotoWidth

		#region DependencyProperty Manager

		public static readonly BindableProperty ManagerProperty = BindableProperty.Create(nameof(Manager),
			typeof(IPhotosManagerViewModel), typeof(PhotosGalleryControl));

		public IPhotosManagerViewModel Manager
		{
			get => (IPhotosManagerViewModel) GetValue(ManagerProperty);
			set => SetValue(ManagerProperty, value);
		}

		#endregion // DependencyProperty Manager

		#region DependencyProperty ShowAddButton

		public static readonly BindableProperty ShowAddButtonProperty = BindableProperty.Create(nameof(ShowAddButton), typeof(bool), typeof(PhotosGalleryControl), true);

		public bool ShowAddButton
		{
			get => (bool) GetValue(ShowAddButtonProperty);
			set => SetValue(ShowAddButtonProperty, value);
		}

		#endregion // DependencyProperty ShowAddButton

		#endregion

		#region ctor

		public PhotosGalleryControl()
		{
			InitializeComponent();
		}

		#endregion

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			PhotoWidth = Math.Ceiling(width / 100.0) > 4
				? new FlexBasis(0.124f, true)
				: new FlexBasis(0.249f, true);

			
		}
	}

	public class AllTrueConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return values?.All(i => i is true) == true;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}