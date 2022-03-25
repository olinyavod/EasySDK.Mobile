using System;
using System.Collections.Generic;

namespace EasySDK.Mobile.ViewModels.Extensions;

public static class CollectionExtensions
{
	#region Public methods

	public static bool TryGetByIndex<TItem>(this IList<TItem> collection, int index, out TItem item)
	{
		if (index >= 0 && index < collection.Count)
		{
			item = collection[index];
			return true;
		}

		item = default;
		return false;
	}

	public static int IndexBy<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> predicate)
	{
		int index = 0;

		foreach (var item in items ?? Array.Empty<TItem>())
		{
			if (predicate?.Invoke(item) == true)
				return index;

			index++;
		}

		return -1;
	}

	#endregion
}