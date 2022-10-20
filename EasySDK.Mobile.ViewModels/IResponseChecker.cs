using System;
using System.Threading.Tasks;
using EasySDK.Mobile.Models;

namespace EasySDK.Mobile.ViewModels;

public interface IResponseChecker
{
	Task<bool> CheckCanContinue(IServiceProvider scope, IResponse response, string defaultErrorMessage);
}