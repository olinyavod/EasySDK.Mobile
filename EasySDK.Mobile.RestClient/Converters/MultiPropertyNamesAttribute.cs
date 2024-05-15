using System;

namespace EasySDK.Mobile.RestClient.Converters;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class MultiPropertyNamesAttribute : Attribute
{
	public string[] Names { get; }

	public MultiPropertyNamesAttribute(params string[] names)
	{
		Names = names;
	}
}