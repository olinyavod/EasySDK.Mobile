namespace EasySDK.Mobile.RestClient;

public interface IQueryBuilder<out TUrl>
{
	TUrl Build();
}