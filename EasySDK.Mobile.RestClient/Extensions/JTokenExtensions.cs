#nullable enable

using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.RestClient.Extensions;

public static class JTokenExtensions
{
	#region Public methods

	/// <summary>Returns the first named scalar whose CLR value is assignable to <typeparamref name="T"/>.</summary>
	/// <remarks>
	/// Returns default(T) when no field matches; use a nullable value type such as long? to distinguish absence.
	/// No conversion is performed: parsed JSON integers are normally Int64, not Int32.
	/// </remarks>
	public static T? GetFirstValue<T>(this JToken token, params string[] fieldNames)
	{
		if (token is not JObject jsonObject)
			return default;

		foreach (var fieldName in fieldNames)
		{
			if (jsonObject.TryGetValue(fieldName, out var value) && value is JValue scalar && scalar.Value is T typedValue)
				return typedValue;
		}

		return default;
	}

	#endregion
}
