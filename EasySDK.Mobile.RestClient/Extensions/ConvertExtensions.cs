#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.RestClient.Extensions
{
	public static class ConvertExtensions
	{
		public static JObject ToJObject<TObject>(this Expression<Func<TObject>> expression)
		{
			var currentExpression = expression.Body;

			var data = new JObject();

			while (currentExpression != null)
			{
				switch (currentExpression)
				{
					case MemberInitExpression initExpr:
						FromBindings(data, initExpr.Bindings);
						currentExpression = null;
						break;

					default:
						currentExpression = null;
						break;
				}
			}

			return data;
		}

		private static void FromBindings(JObject data, ICollection<MemberBinding> bindings)
		{
			foreach (var binding in bindings)
			{
				var propertyName = GetPropertyName(binding.Member);

				switch (binding)
				{
					case MemberAssignment assignment:
					{
						var value = Expression.Lambda<Func<object?>>
						(
							Expression.Convert(assignment.Expression, typeof(object))
						).Compile()();

						data[propertyName] = TokenFromValue(assignment.Member, value);

						break;
					}

					case MemberMemberBinding:
						
						break;
				}
			}
		}

		private static JToken? TokenFromValue(MemberInfo member, object? value)
		{
			if (value != null
			    && member.GetCustomAttribute<JsonConverterAttribute>() is { } converterFactory
			    && Activator.CreateInstance(converterFactory.ConverterType) is JsonConverter
			    {
				    CanWrite: true
			    } converter
			    && converter.CanConvert(value.GetType()))
			{
				using JTokenWriter writer = new();
				converter.WriteJson(writer, value, new JsonSerializer());

				return writer.CurrentToken;
			}

			return value != null ? JToken.FromObject(value) : null;
		}

		private static string GetPropertyName(MemberInfo member)
		{
			if(member.GetCustomAttribute<JsonPropertyAttribute>() is { } jatt)
				return jatt.PropertyName ?? member.Name;

			return member.Name;
		}
	}
}
