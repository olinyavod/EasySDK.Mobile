using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.RestClient.Converters;

class ErrorsJsonConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, value);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var result = existingValue as Dictionary<string, IEnumerable<string>>;

		result ??= new Dictionary<string, IEnumerable<string>>();

		var token = JToken.ReadFrom(reader);

		foreach (var item in token.Children())
		{
			switch (item)
			{
				case JProperty {Value: {Type: JTokenType.Array} arrayToken}:
					result[item.Path] = arrayToken.Value<string[]>();
					break;

				case JProperty {Value: {Type: JTokenType.String} messageToken}:
					result[item.Path] = new[] {messageToken.Value<string>()};
					break;
			}
		}

		return result;
	}

	public override bool CanConvert(Type objectType) => objectType == typeof(Dictionary<string, IEnumerable<string>>);
}