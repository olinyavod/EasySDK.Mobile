using System;
using System.IO;

namespace EasySDK.Mobile.RestClient.Models;

public class HttpPostFileRequest : IProgressRequest
{
	#region Events

	public event EventHandler<FileProgressEventArgs> ProgressChanged;

	#endregion

	#region Properties

	public string FileName { get; }

	public Stream FileStream { get; }

	public string FileType { get; }

	#endregion

	#region ctor

	public HttpPostFileRequest(string fileName, string fileType, Stream fileStream)
	{
		FileName = fileName;
		FileStream = fileStream;
		FileType = fileType;
	}

	#endregion

	#region Public methods

	void IProgressRequest.RaiseProgressChanged(long currentBytes, long totalBytes)
	{
		ProgressChanged?.Invoke(this, new FileProgressEventArgs(currentBytes, totalBytes));
	}

	#endregion
}