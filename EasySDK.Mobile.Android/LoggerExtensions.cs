using EasySDK.Mobile.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Fluent;
using NLog.Targets;

namespace EasySDK.Mobile.Android;

public static class LoggerExtensions
{
	public static LoggingConfiguration CreateDefaultLogConfig(this IPathsService pathsService)
	{
		var config = new NLog.Config.LoggingConfiguration();
		var logFIle = pathsService.GetLogsFilePath();

		var logFile = new FileTarget("logfile")
		{
			FileName         = logFIle,
			Layout           = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
			MaxArchiveFiles  = 3,
			ArchiveFileKind  = FilePathKind.Relative,
			ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
			ArchiveEvery     = FileArchivePeriod.Day
		};
		var logConsole = new ConsoleTarget("logconsole")
		{
			Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=tostring}",
		};

		config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
		config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

		return config;
	}

	public static void DefaultNLogsConfigure(this IPathsService pathsService)
	{
		LogManager.Configuration = pathsService.CreateDefaultLogConfig();
	}

	public static void AddDefaultNLog(this IServiceCollection services)
	{
		services.AddLogging(b =>
		{
			b.AddNLog(c =>
			{
				var result = new LogFactory();

				var pathsService = c.GetService<IPathsService>();

				var config = pathsService.CreateDefaultLogConfig();

				result.Setup().LoadConfiguration(config);

				return result;
			});
		});
	}
}