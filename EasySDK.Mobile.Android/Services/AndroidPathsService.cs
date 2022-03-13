using System;
using System.IO;
using EasySDK.Mobile.ViewModels.Services;

namespace EasySDK.Mobile.Android.Services;

public class DefaultAndroidPathsService : IPathsService
{
	public string GetLogsFilePath()
	{
		var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

		return Path.Combine(personalFolder, "Logs", "logs.log");
	}
}