namespace EasySDK.Mobile.RestClient.Models;

public interface IProgressRequest
{
	#region Methods

	void RaiseProgressChanged(long currentBytes, long totalBytes);

	#endregion
}