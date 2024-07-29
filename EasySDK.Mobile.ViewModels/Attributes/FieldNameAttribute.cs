using System;

namespace EasySDK.Mobile.ViewModels.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldNameAttribute : Attribute
{
	public string Name { get; }

	public FieldNameAttribute(string name)
	{
		Name = name;
	}
}