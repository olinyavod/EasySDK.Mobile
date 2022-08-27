using System;
using System.Collections.Generic;

namespace EasySDK.Mobile.ViewModels.Converters;

public class IgnoreCaseComparer : IEqualityComparer<string>
{
	public bool Equals(string x, string y)
	{
		return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase) == 0;
	}

	public int GetHashCode(string obj)
	{
		return obj.GetHashCode();
	}
}