using EasySDK.Mobile.ViewModels;
using Xamarin.Forms;

[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", FormsApp.BaseNamespace)]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Pages)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Extensions)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Effects)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Controls)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Behaviors)}")]
[assembly:XmlnsDefinition("http://easyptog.ru/sdk/mobile", $"{FormsApp.BaseNamespace}.{nameof(EasySDK.Mobile.ViewModels.Converters)}")]