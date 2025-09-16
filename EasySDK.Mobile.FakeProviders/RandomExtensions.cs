using System;
using System.Linq;
using AutoFixture;

namespace EasySDK.Mobile.FakeProviders;

public static class RandomExtensions
{
	private static readonly Random _random = new();

	public static TItem GetRandomItem<TItem>(this TItem[] items) => items
		.Skip(_random.Next(0, items.Length))
		.FirstOrDefault();

	public static int RandomTo(this int min, int max) => _random.Next(min, max);

	public static T CreateFromList<T>(this IFixture fixture, T[] items) => items
		.Skip(_random.Next(0, items.Length))
		.FirstOrDefault();

}