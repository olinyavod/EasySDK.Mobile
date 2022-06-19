using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EasySDK.Mobile.Android;

public class NLogTraceListener : TraceListener
{
	private readonly ILogger _logger;

	public NLogTraceListener(ILogger logger)
	{
		_logger = logger;
	}

	public override void Write(string message)
	{
		_logger.LogTrace(message);
	}

	public override void WriteLine(string message)
	{
		_logger.LogTrace(message);
	}
}