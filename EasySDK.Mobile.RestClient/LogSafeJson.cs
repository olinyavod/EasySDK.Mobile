#nullable enable

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EasySDK.Mobile.RestClient;

/// <summary>
/// Serializes objects to JSON for logging purposes only. Properties marked
/// with <see cref="SensitiveAttribute"/> are replaced with a fixed mask
/// (<c>"***"</c>); <c>null</c> and empty strings are passed through so it
/// is still visible whether a value was set at all.
///
/// IMPORTANT: never use this for the actual HTTP payload — it loses data.
/// </summary>
public static class LogSafeJson
{
	private const string Mask = "***";

	private static readonly JsonSerializerSettings Settings = new()
	{
		ContractResolver = new SensitiveContractResolver()
	};

	public static string Serialize(object? value)
		=> value == null ? "null" : JsonConvert.SerializeObject(value, Settings);

	private sealed class SensitiveContractResolver : DefaultContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if (member.GetCustomAttribute<SensitiveAttribute>(inherit: true) != null && property.ValueProvider != null)
				property.ValueProvider = new MaskedValueProvider(property.ValueProvider);

			return property;
		}
	}

	private sealed class MaskedValueProvider : IValueProvider
	{
		private readonly IValueProvider _inner;

		public MaskedValueProvider(IValueProvider inner) => _inner = inner;

		public object? GetValue(object target)
		{
			var raw = _inner.GetValue(target);
			return raw switch
			{
				null                        => null,
				string s when s.Length == 0 => s,
				_                           => Mask
			};
		}

		public void SetValue(object target, object? value) => _inner.SetValue(target, value);
	}
}
