using EasySDK.Mobile.ViewModels;
using Xamarin.Forms;

[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", Constants.BaseNamespace)]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{Constants.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Extensions)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{Constants.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Converters)}")]