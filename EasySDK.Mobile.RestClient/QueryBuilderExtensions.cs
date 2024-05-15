using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySDK.Mobile.RestClient;

public static class QueryBuilderExtensions
{
	class QueryStringBuilder : IQueryBuilder<string>
	{
		private readonly string                     _baseUrl;
		private readonly Dictionary<string, string> _params = new();

		public QueryStringBuilder(string baseUrl)
		{
			_baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
		}

		public void AddQuery(string name, string? value)
		{
			if(string.IsNullOrEmpty(value))
				return;

			_params[name] = Uri.EscapeDataString(value);
		}

		public string Build()
		{
			var query = string.Join("&", _params.Select(i => $"{i.Key}={i.Value}"));

			return string.IsNullOrWhiteSpace(query) ? _baseUrl : $"{_baseUrl}?{query}";
		}
	}

	public static IQueryBuilder<string> CreateQueryBuilder(this string query)
		=> new QueryStringBuilder(query);

	public static IQueryBuilder<string> AddQuery(this string query, string name, string? value)
	{
		var builder = query.CreateQueryBuilder();

		builder.AddQuery(name, value);

		return builder;
	}

	public static IQueryBuilder<string> AddQuery(this IQueryBuilder<string> builder, string name, string? value)
	{
		if (builder is QueryStringBuilder sb)
			sb.AddQuery(name, value);

		return builder;
	}
}