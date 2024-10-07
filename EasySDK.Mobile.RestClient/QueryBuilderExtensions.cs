#nullable enable 

namespace EasySDK.Mobile.RestClient;

public static class QueryBuilderExtensions
{
	public static QueryStringBuilder CreateQueryBuilder(this string query)
		=> new QueryStringBuilder(query);

	public static QueryStringBuilder AddQuery(this string query, string name, string? value)
	{
		var builder = query.CreateQueryBuilder();

		builder.AddQuery(name, value);

		return builder;
	}
}