using System;
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
}