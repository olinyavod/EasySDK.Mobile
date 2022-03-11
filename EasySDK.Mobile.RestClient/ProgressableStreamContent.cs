using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EasySDK.Mobile.RestClient.Models;

namespace EasySDK.Mobile.RestClient;

public class ProgressableStreamContent : HttpContent
{
	#region Private fields

	private const int DefaultBufferSize = 4096;

	private readonly Stream _content;
	private readonly int _bufferSize;
	private readonly IProgressRequest _downloader;

	#endregion

	#region ctor

	public ProgressableStreamContent(Stream content, IProgressRequest downloader)
		: this(content, DefaultBufferSize, downloader)
	{

	}

	public ProgressableStreamContent
	(
		Stream content,
		int bufferSize,
		IProgressRequest downloader
	)
	{
		_content = content ?? throw new ArgumentNullException(nameof(content));
		_bufferSize = bufferSize;
		_downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
	}

	#endregion

	#region Protected methods

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
	{
		return Task.Run( async () =>
		{
			var buffer = new byte[this._bufferSize];
			var size = _content.Length;
			var uploaded = 0;

			while ((await _content.ReadAsync(buffer, 0, buffer.Length)) is { } length
			       && length > 0)
			{
	            
				uploaded += length;

				await stream.WriteAsync(buffer, 0, length);
				await stream.FlushAsync();

				_downloader.RaiseProgressChanged(uploaded, size);
			}
		});
	}

	protected override bool TryComputeLength(out long length)
	{
		length = _content.Length;
		return true;
	}

	protected override void Dispose(bool disposing)
	{
		if(disposing)
			_content.Dispose();

		base.Dispose(disposing);
	}

	#endregion
}