using System;
using System.Collections.Generic;
using System.Linq;

namespace EasySDK.Mobile.RestClient;

public class QueryStringBuilder : IQueryBuilder<string>
{
	private readonly string                     _baseUrl;
	private readonly Dictionary<string, string> _params = new();

	public QueryStringBuilder(string baseUrl)
	{
		_baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
	}

	public QueryStringBuilder AddQuery(string name, string value)
	{
		if(string.IsNullOrEmpty(value))
			return this;

		_params[name] = Uri.EscapeDataString(value);

		return this;
	}

	public string Build()
	{
		var query = string.Join("&", _params.Select(i => $"{i.Key}={i.Value}"));

		return string.IsNullOrWhiteSpace(query) ? _baseUrl : $"{_baseUrl}?{query}";
	}

	public static implicit operator string(QueryStringBuilder builder) => builder.Build();
}