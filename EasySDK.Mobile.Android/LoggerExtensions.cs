using EasySDK.Mobile.ViewModels.Services;
using NLog;
using NLog.Targets;

namespace EasySDK.Mobile.Android;

public static class LoggerExtensions
{
	public static void DefaultNLogsConfigure(this IPathsService pathsService)
	{
		var config = new NLog.Config.LoggingConfiguration();
		var logFile = new FileTarget("logfile")
		{
			FileName = pathsService.GetLogsFilePath(),
			Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
			MaxArchiveFiles = 3,
			ArchiveFileKind = FilePathKind.Relative,
			ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
			ArchiveEvery = FileArchivePeriod.Day
		};
		var logConsole = new ConsoleTarget("logconsole")
		{
			Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
		};

		config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
		config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

		LogManager.Configuration = config;
	}
}