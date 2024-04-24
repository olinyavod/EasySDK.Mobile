using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using EasySDK.Mobile.Forms.Extensions;
using EasySDK.Mobile.Models;
using EasySDK.Mobile.ViewModels;
using EasySDK.Mobile.ViewModels.Input;
using FFImageLoading;
using FFImageLoading.Work;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;
using Xamarin.Forms;
using ImageSource = Xamarin.Forms.ImageSource;

namespace EasySDK.Mobile.Forms.Managers
{

	public abstract class PhotosManagerViewModelBase<TMediaFile> : ViewModelBase, IPhotosManagerViewModel
		where TMediaFile : class, IMediaFile
	{
		#region Private fields

		private readonly IUserDialogs _dialogs;
		private readonly IImageService _imageService;
		private readonly ILogger _logger;

		private bool _isPhotoOpened;
		private PhotoItemViewModel? _selectedPhoto;
		private int _photosCount;
		private int _selectedPhotoIndex;

		#endregion

		#region Properties

		public abstract bool AllowDelete { get; }

		public abstract bool AllowAdd { get; }

		protected virtual bool AllowGallerySource { get; } = false;

		public ObservableCollection<PhotoItemViewModel> PhotosSource { get; } = new();

		public bool IsPhotoOpened
		{
			get => _isPhotoOpened;
			set => SetProperty(ref _isPhotoOpened, value);
		}

		public PhotoItemViewModel? SelectedPhoto
		{
			get => _selectedPhoto;
			set => SetProperty(ref _selectedPhoto, value, SelectedPhotoOnChanged);
		}

		public int PhotosCount
		{
			get => _photosCount;
			set => SetProperty(ref _photosCount, value);
		}

		public int SelectedPhotoIndex
		{
			get => _selectedPhotoIndex;
			set => SetProperty(ref _selectedPhotoIndex, value);
		}

		#endregion

		#region ctor

		protected PhotosManagerViewModelBase
		(
			IServiceScopeFactory scopeFactory,
			IUserDialogs         dialogs,
			IImageService        imageService,
			ILogger              logger
		) : base(scopeFactory)
		{
			_dialogs      = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
			_imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
			_logger       = logger ?? throw new ArgumentNullException(nameof(logger));

			PhotosSource.CollectionChanged += PhotosSourceOnCollectionChanged;

			AddPhotoCommand    = new AsyncCommand(OnAddPhoto, OnCanAddPhoto);
			DeletePhotoCommand = new AsyncCommand<PhotoItemViewModel?>(OnDeletePhoto, OnCanDeletePhoto);
			OpenPhotoCommand   = new Command<PhotoItemViewModel?>(OnOpenPhoto, OnCanOpenPhoto);
			ClosePhotoCommand  = new Command(OnClosePhoto, OnCanClosePhoto);
		}

		#endregion

		#region Public methods

		public void SetPhotos(IEnumerable<TMediaFile>? photos)
		{
			PhotosSource.Clear();

			foreach (var p in photos ?? Enumerable.Empty<TMediaFile>())
				PhotosSource.Add(new PhotoItemViewModel
				{
					Id = p.Id, ImageSource = CreateImageSource(p.Url)
				});
		}

		#endregion

		#region Protected methods

		private async Task<Stream> TryLoadContent(string url, CancellationToken cancellationToken)
		{
			try
			{
				if (_imageService.Config.HttpClient is { } client)
				{
					using var response = await client.GetAsync(url, cancellationToken);

					var buffer = await response.Content.ReadAsByteArrayAsync();

					return new MemoryStream(buffer);
				}

				return await _imageService.LoadUrl(url).AsPNGStreamAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Load photo error.");
				return Stream.Null;
			}
		}

		protected virtual ImageSource? CreateImageSource(string url) => Device.RuntimePlatform switch
		{
			Device.iOS => ImageSource.FromStream(t => TryLoadContent(url, t)),
			_ when Uri.TryCreate(url, UriKind.Absolute, out var u) => ImageSource.FromUri(u),

			_ => null
		};

		protected abstract Task<IResponse<TMediaFile>> AddPhotoAsync(IServiceProvider scope, string fileName,
			Stream fileContent);

		protected abstract Task<IResponse<bool?>> DeletePhotoAsync(IServiceProvider scope, string id);

		protected virtual async Task<FileResult?> GetPhotoAsync()
		{
			if (!AllowGallerySource)
				return await TryCapturePhotoAsync();

			var result = await _dialogs.ActionSheetAsync
			(
				ViewModels.Properties.Resources.WhereTakePhoto,
				ViewModels.Properties.Resources.Cancel,
				null,
				null,
				ViewModels.Properties.Resources.Camera, ViewModels.Properties.Resources.Gallery);

			if (result == ViewModels.Properties.Resources.Camera)
				return await TryCapturePhotoAsync();

			if (result == ViewModels.Properties.Resources.Gallery)
				return await TryPickPhotoAsync();

			return null;
		}

		protected virtual Task ShowResponseError(IResponse response, string defaultMessage)
		{
			_dialogs.ShowErrorMessage(defaultMessage);
			return Task.CompletedTask;
		}

		#endregion

		#region Private methods

		private void PhotosSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			PhotosCount = PhotosSource.Count;
		}

		private void SelectedPhotoOnChanged()
		{
			SelectedPhotoIndex = SelectedPhoto is { } photo
				? PhotosSource.IndexOf(photo) + 1
				: 0;
		}

		private async Task<FileResult?> TryCapturePhotoAsync()
		{
			try
			{
				if (!await _dialogs.RequestPermissionIfDeny<Xamarin.Essentials.Permissions.Camera>
				    (
					    ViewModels.Properties.Resources.NeedAllowAccessCameraMessage
				    ))
					return null;

				return await MediaPicker.CapturePhotoAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Capture photo error.");
				return null;
			}

		}

		private async Task<FileResult?> TryPickPhotoAsync()
		{
			try
			{
				if (!await _dialogs.RequestPermissionIfDeny<Xamarin.Essentials.Permissions.Photos>
				    (
					    ViewModels.Properties.Resources.NeedAllowAccessCameraMessage
				    ))
					return null;

				return await MediaPicker.PickPhotoAsync(new MediaPickerOptions());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Pick photo error.");
				return null;
			}
		}

		#endregion

		#region AddPhotoCommand

		public ICommand AddPhotoCommand { get; }

		private bool OnCanAddPhoto() => true;

		private async Task OnAddPhoto()
		{
			try
			{

				var fileResult = await GetPhotoAsync();
				if (fileResult == null)
					return;

				await Task.Delay(500);

				using var loadingDlh = _dialogs.Loading(ViewModels.Properties.Resources.Saving);
				await using var scope = CreateAsyncScope();

				var maxSize = Device.RuntimePlatform switch
				{
					Device.iOS => 1024.0 / DeviceDisplay.MainDisplayInfo.Density,

					_ => 1024.0
				};
				await using var stream = await _imageService.LoadStream(_ => fileResult.OpenReadAsync())
					.DownSample((int) maxSize, (int) maxSize)
					.DownSampleMode(InterpolationMode.Medium)
					.AsJPGStreamAsync(50);

				await _imageService.LoadStream(async t =>
				{
					var memory = new MemoryStream();
					await stream.CopyToAsync(memory, t);
					memory.Seek(0, SeekOrigin.Begin);
					return memory;
				}).Success((info, _) =>
				{
					_logger.LogInformation("Try send photo with size: {0}x{1}", info.OriginalWidth,
						info.OriginalHeight);
				}).PreloadAsync();

				stream.Seek(0, SeekOrigin.Begin);

				var response = await AddPhotoAsync(scope.ServiceProvider, fileResult.FileName, stream);

				if (response.HasError)
				{
					await ShowResponseError(response, ViewModels.Properties.Resources.FailedAddPhotoMessage);
					return;
				}

				var photo = response.Result;
				var item = new PhotoItemViewModel
				{
					ImageSource = CreateImageSource(photo.Url),
					Id = photo.Id
				};
				PhotosSource.Add(item);

				SelectedPhoto = item;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "AddAsync photo error.");
				_dialogs.ShowErrorMessage(ViewModels.Properties.Resources.FailedAddPhotoMessage);
			}
		}

		#endregion //AddPhotoCommand

		#region DeletePhotoCommand

		public ICommand DeletePhotoCommand { get; }

		private bool OnCanDeletePhoto(PhotoItemViewModel? item) => item != null;

		private async Task OnDeletePhoto(PhotoItemViewModel? item)
		{
			try
			{
				if (!await _dialogs.ShowConfirmAsync
				    (
					    ViewModels.Properties.Resources.DeletePhotoQuestion,
					    ViewModels.Properties.Resources.Photos
				    ))
					return;

				using var loadingDlg = _dialogs.Loading(ViewModels.Properties.Resources.Loading);
				await using var scope = CreateAsyncScope();

				var response = await DeletePhotoAsync(scope.ServiceProvider, item!.Id);

				if (response.HasError)
				{
					await ShowResponseError(response, ViewModels.Properties.Resources.FailedDeletePhotoMessage);
					return;
				}

				PhotosSource.Remove(item);

				if (PhotosSource.Count == 0)
					IsPhotoOpened = false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete photo error.");

				_dialogs.ShowErrorMessage(ViewModels.Properties.Resources.FailedDeletePhotoMessage);
			}
		}

		#endregion //DeletePhotoCommand

		#region OpenPhotoCommand

		public ICommand OpenPhotoCommand { get; }

		private bool OnCanOpenPhoto(PhotoItemViewModel? item) => item != null;

		private void OnOpenPhoto(PhotoItemViewModel? item)
		{
			SelectedPhoto = item;
			IsPhotoOpened = true;
		}

		#endregion //OpenPhotoCommand

		#region ClosePhotoCommand

		public ICommand ClosePhotoCommand { get; }

		private bool OnCanClosePhoto() => true;

		private void OnClosePhoto()
		{
			IsPhotoOpened = false;
		}

		#endregion //ClosePhotoCommand
	}
}