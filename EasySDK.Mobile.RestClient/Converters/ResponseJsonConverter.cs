using System;
using System.Linq;
using EasySDK.Mobile.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EasySDK.Mobile.RestClient.Converters;

public class ResponseJsonConverter:JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var token = serializer.Deserialize<JToken>(reader);
		var contract = (JsonObjectContract) serializer.ContractResolver.ResolveContract(objectType)!;
		var result = contract.DefaultCreator!();

		foreach (var property in contract.Properties.Where(i => i.Writable && !i.Ignored))
		{
			if (GetAltNames(property) is { } names 
			    && names.Select(i => token[i]).FirstOrDefault(i => i != null) is { } t)
			{
				var value = GetValue(t, serializer, property);
				property.ValueProvider?.SetValue(result, value);
			}
			else if(token[property.PropertyName!] is { } pt)
			{
				var value = GetValue(pt, serializer, property);
				property.ValueProvider?.SetValue(result, value);
			}
		}

		return result;
	}

	private object? GetValue(JToken token, JsonSerializer serializer, JsonProperty property)
	{
		try
		{
			return token.ToObject(property.PropertyType, serializer);
		}
		catch
		{
			return property.DefaultValue;
		}
	} 

	private static string[]? GetAltNames(JsonProperty property)
	{
		var att = property.AttributeProvider?
			.GetAttributes(typeof(MultiPropertyNamesAttribute), true)
			.FirstOrDefault() as MultiPropertyNamesAttribute;

		return att?.Names;
	}

	public override bool CanWrite { get; } = false;

	public override bool CanRead { get; } = true;

	public override bool CanConvert(Type objectType) => objectType.IsInstanceOfType(typeof(IResponse));
}