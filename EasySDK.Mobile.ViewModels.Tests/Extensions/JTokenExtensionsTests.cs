using EasySDK.Mobile.RestClient.Extensions;
using Newtonsoft.Json.Linq;

namespace EasySDK.Mobile.ViewModels.Tests.Extensions;

public class JTokenExtensionsTests
{
	[TestCase("first", "second", "First")]
	[TestCase("second", "first", "Second")]
	[TestCase("missing", "second", "Second")]
	public void GetFirstValue_uses_requested_field_names_and_order(string firstField, string secondField, string expected)
	{
		var token = JToken.Parse(@"{""first"":""First"",""second"":""Second""}");

		Assert.That(token.GetFirstValue<string>(firstField, secondField), Is.EqualTo(expected));
	}

	[TestCase(@"{""candidate"":{}}")]
	[TestCase(@"{""candidate"":[]}")]
	[TestCase(@"{""candidate"":42}")]
	[TestCase(@"{""candidate"":1.5}")]
	[TestCase(@"{""candidate"":true}")]
	[TestCase(@"{""candidate"":false}")]
	[TestCase(@"{""candidate"":null}")]
	[TestCase("{}")]
	public void GetFirstValue_skips_missing_null_and_non_string_fields(string content)
	{
		var token = JObject.Parse(content);
		token["fallback"] = "Fallback";

		Assert.That(token.GetFirstValue<string>("candidate", "fallback"), Is.EqualTo("Fallback"));
		Assert.That(token.GetFirstValue<string>("candidate"), Is.Null);
	}

	[TestCase("")]
	[TestCase(" \t ")]
	public void GetFirstValue_preserves_empty_or_whitespace_strings(string value)
	{
		var token = new JObject { ["candidate"] = value, ["fallback"] = "Fallback" };

		Assert.That(token.GetFirstValue<string>("candidate", "fallback"), Is.EqualTo(value));
	}

	[Test]
	public void GetFirstValue_returns_default_without_field_names()
	{
		var token = JToken.Parse(@"{""message"":""Message""}");

		Assert.That(token.GetFirstValue<string>(), Is.Null);
		Assert.That(token.GetFirstValue<long>(), Is.Zero);
		Assert.That(token.GetFirstValue<bool>(), Is.False);
		Assert.That(token.GetFirstValue<long?>(), Is.Null);
	}

	[Test]
	public void GetFirstValue_is_case_sensitive()
	{
		var token = JToken.Parse(@"{""Candidate"":""Different case"",""fallback"":""Fallback""}");

		Assert.That(token.GetFirstValue<string>("candidate", "fallback"), Is.EqualTo("Fallback"));
		Assert.That(token.GetFirstValue<string>("candidate"), Is.Null);
	}

	[Test]
	public void GetFirstValue_does_not_search_descendants()
	{
		var token = JToken.Parse(@"{""nested"":{""candidate"":""Nested""},""fallback"":""Fallback""}");

		Assert.That(token.GetFirstValue<string>("candidate", "fallback"), Is.EqualTo("Fallback"));
		Assert.That(token.GetFirstValue<string>("candidate"), Is.Null);
	}

	[TestCase(@"[{""candidate"":""Nested""}]")]
	[TestCase("[]")]
	[TestCase(@"""Root string""")]
	[TestCase("42")]
	[TestCase("1.5")]
	[TestCase("true")]
	[TestCase("null")]
	public void GetFirstValue_returns_default_for_non_object_roots(string content)
	{
		var token = JToken.Parse(content);

		Assert.That(token.GetFirstValue<string>("candidate"), Is.Null);
		Assert.That(token.GetFirstValue<long?>("candidate"), Is.Null);
		Assert.That(token.GetFirstValue<long>("candidate"), Is.Zero);
	}

	[TestCase(0L)]
	[TestCase(42L)]
	public void GetFirstValue_matches_parsed_integer_as_long(long expected)
	{
		var token = JToken.Parse(expected == 0 ? @"{""candidate"":0,""fallback"":99}" : @"{""candidate"":42,""fallback"":99}");

		Assert.That(token.GetFirstValue<long>("candidate", "fallback"), Is.EqualTo(expected));
		Assert.That(token.GetFirstValue<long?>("candidate", "fallback"), Is.EqualTo(expected));
	}

	[Test]
	public void GetFirstValue_matches_parsed_double_without_converting_integer_or_string()
	{
		var token = JToken.Parse(@"{""integer"":42,""text"":""1.5"",""candidate"":1.5}");

		Assert.That(token.GetFirstValue<double>("integer", "text", "candidate"), Is.EqualTo(1.5));
		Assert.That(token.GetFirstValue<double?>("integer", "text"), Is.Null);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void GetFirstValue_matches_parsed_boolean_including_false(bool expected)
	{
		var token = JToken.Parse(expected ? @"{""candidate"":true,""fallback"":false}" : @"{""candidate"":false,""fallback"":true}");

		Assert.That(token.GetFirstValue<bool>("candidate", "fallback"), Is.EqualTo(expected));
		Assert.That(token.GetFirstValue<bool?>("candidate", "fallback"), Is.EqualTo(expected));
	}

	[TestCase("{}")]
	[TestCase(@"{""candidate"":null}")]
	[TestCase(@"{""candidate"":{}}")]
	[TestCase(@"{""candidate"":[]}")]
	[TestCase(@"{""candidate"":""0""}")]
	public void GetFirstValue_returns_null_for_absent_or_incompatible_nullable_values(string content)
	{
		var token = JToken.Parse(content);

		Assert.That(token.GetFirstValue<long?>("candidate"), Is.Null);
		Assert.That(token.GetFirstValue<bool?>("candidate"), Is.Null);
		Assert.That(token.GetFirstValue<long>("candidate"), Is.Zero);
		Assert.That(token.GetFirstValue<bool>("candidate"), Is.False);
	}

	[Test]
	public void GetFirstValue_does_not_convert_parsed_long_to_int()
	{
		var token = JObject.Parse(@"{""parsed"":42}");
		token["intValue"] = new JValue((object) 7);

		Assert.That(token.GetFirstValue<int?>("parsed"), Is.Null);
		Assert.That(token.GetFirstValue<int>("parsed", "intValue"), Is.EqualTo(7));
		Assert.That(token.GetFirstValue<long>("intValue", "parsed"), Is.EqualTo(42L));
	}

	[TestCase(@"{""candidate"":{}}")]
	[TestCase(@"{""candidate"":[]}")]
	[TestCase(@"{""candidate"":null}")]
	public void GetFirstValue_skips_objects_arrays_and_null_even_when_target_is_object(string content)
	{
		var token = JObject.Parse(content);
		token["fallback"] = "Fallback";

		Assert.That(token.GetFirstValue<object>("candidate"), Is.Null);
		Assert.That(token.GetFirstValue<object>("candidate", "fallback"), Is.EqualTo("Fallback"));
	}

	[Test]
	public void GetFirstValue_returns_assignable_scalar_object_in_requested_order()
	{
		var token = JToken.Parse(@"{""integer"":42,""text"":""Text""}");

		Assert.That(token.GetFirstValue<object>("integer", "text"), Is.TypeOf<long>().And.EqualTo(42L));
		Assert.That(token.GetFirstValue<object>("text", "integer"), Is.TypeOf<string>().And.EqualTo("Text"));
	}
}
