using System;

namespace EasySDK.Mobile.RestClient;

/// <summary>
/// Marks a property as sensitive: its value will be replaced with a mask
/// when the containing object is serialized for logging via
/// <see cref="LogSafeJson"/>.
///
/// Has NO effect on the regular JSON serialization that produces the HTTP
/// request body — the network always sees the real value.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class SensitiveAttribute : Attribute
{
}
