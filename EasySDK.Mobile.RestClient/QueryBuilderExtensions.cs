#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

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

	public static QueryStringBuilder AddEnum<TValue>(this string query, string name, IEnumerable<TValue>? value)
		where TValue : Enum
	{
		var builder = query.CreateQueryBuilder();

		builder.AddEnum(name, value);

		return builder;
	}

	public static QueryStringBuilder AddEnum<TRequest, TValue>(this string query, object request, string key, Func<TRequest, IEnumerable<TValue>?> values)
		where TValue : Enum
	{
		var builder = query.CreateQueryBuilder();

		return builder.AddEnum<TRequest, TValue>(request, key, values);
	}

	public static QueryStringBuilder AddEnum<TRequest, TValue>(this QueryStringBuilder builder, object request, string key, Func<TRequest, IEnumerable<TValue>?> values)
		where TValue : Enum
	{
		if (request is not TRequest r)
			return builder;

		return builder.AddEnum(key, values(r));
	}

	public static QueryStringBuilder AddEnum<TValue>(this QueryStringBuilder builder, string key, IEnumerable<TValue>? values)
		where TValue : Enum
	{
		if (values == null)
			return builder;

		var enumTypes = typeof(TValue);

		var query = string.Join(",", values.Select(i => GetEnumValueName(i)));

		builder.AddQuery(key, query);

		return builder;
	}

	public static string GetEnumValueName(object value)
	{
		var enumType = value.GetType();
		var valueName = Enum.GetName(enumType, value)!;

		if (enumType.GetField(valueName)?.GetCustomAttribute<EnumMemberAttribute>() is {Value: { } enumMember})
			valueName = enumMember;

		return valueName;
	}

	public static QueryStringBuilder AddEnumQuery<TRequest>
	(
		this QueryStringBuilder builder,
		string                  name,
		object                  request,
		Func<TRequest, object?> value
	) where TRequest : class
	{
		if (request is not TRequest r)
			return builder;

		if (value(r) is not { } v)
			return builder;

		var valueName = QueryBuilderExtensions.GetEnumValueName(v);
		
		builder.AddQuery(name, $"{valueName}");

		return builder;
	}

	public static QueryStringBuilder AddEnumQuery<TValue>
	(
		this QueryStringBuilder builder,
		string                  name,
		TValue?                 value
	)
	{
		if (value == null)
			return builder;

		var valueName = QueryBuilderExtensions.GetEnumValueName(value);
		
		builder.AddQuery(name, $"{valueName}");

		return builder;
	}
}