using System;

namespace EasySDK.Mobile.RestClient.Models;

public class FileProgressEventArgs : EventArgs
{
	#region Properties

	public long CurrentBytes { get; }

	public long TotalBytes { get; }

	#endregion

	#region ctor

	public FileProgressEventArgs(long currentBytes, long totalBytes)
	{
		CurrentBytes = currentBytes;
		TotalBytes = totalBytes;
	}

	#endregion
}