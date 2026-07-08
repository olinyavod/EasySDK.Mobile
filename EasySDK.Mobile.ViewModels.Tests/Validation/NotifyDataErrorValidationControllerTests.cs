using System.Collections;
using System.ComponentModel;
using EasySDK.Mobile.ViewModels.Validation;

namespace EasySDK.Mobile.ViewModels.Tests.Validation;

public class NotifyDataErrorValidationControllerTests
{
	[Test]
	public void Register_applies_existing_error()
	{
		var source = new TestErrorsSource();
		source.SetErrors("Name", "Required");
		var target = new TestTarget();

		using var controller = CreateController();
		controller.SetErrorsSource(source);
		controller.Register("Name", target);

		Assert.That(target.HasError, Is.True);
		Assert.That(target.ErrorText, Is.EqualTo("Required"));
	}

	[Test]
	public void ErrorsChanged_updates_registered_target()
	{
		var source = new TestErrorsSource();
		var target = new TestTarget();

		using var controller = CreateController();
		controller.SetErrorsSource(source);
		controller.Register("Name", target);

		source.SetErrors("Name", "Required");

		Assert.That(target.HasError, Is.True);
		Assert.That(target.ErrorText, Is.EqualTo("Required"));

		source.SetErrors("Name");

		Assert.That(target.HasError, Is.False);
		Assert.That(target.ErrorText, Is.Null);
	}

	[Test]
	public void ErrorsChanged_with_empty_property_name_updates_all_targets()
	{
		var source = new TestErrorsSource();
		var first = new TestTarget();
		var second = new TestTarget();

		using var controller = CreateController();
		controller.SetErrorsSource(source);
		controller.Register("FirstName", first);
		controller.Register("SecondName", second);

		source.SetErrorsWithoutNotification("FirstName", "First required");
		source.SetErrorsWithoutNotification("SecondName", "Second required");
		source.RaiseErrorsChanged(string.Empty);

		Assert.That(first.ErrorText, Is.EqualTo("First required"));
		Assert.That(second.ErrorText, Is.EqualTo("Second required"));
	}

	[Test]
	public void SetErrorsSource_detaches_previous_source()
	{
		var oldSource = new TestErrorsSource();
		var newSource = new TestErrorsSource();
		var target = new TestTarget();

		using var controller = CreateController();
		controller.SetErrorsSource(oldSource);
		controller.Register("Name", target);
		controller.SetErrorsSource(newSource);

		oldSource.SetErrors("Name", "Old error");
		Assert.That(target.HasError, Is.False);
		Assert.That(target.ErrorText, Is.Null);

		newSource.SetErrors("Name", "New error");
		Assert.That(target.HasError, Is.True);
		Assert.That(target.ErrorText, Is.EqualTo("New error"));
		Assert.That(oldSource.RemoveCount, Is.EqualTo(1));
	}

	[Test]
	public void Dispose_detaches_errors_source()
	{
		var source = new TestErrorsSource();
		var target = new TestTarget();

		var controller = CreateController();
		controller.SetErrorsSource(source);
		controller.Register("Name", target);
		controller.Dispose();

		source.SetErrors("Name", "Required");

		Assert.That(target.HasError, Is.False);
		Assert.That(target.ErrorText, Is.Null);
		Assert.That(source.RemoveCount, Is.EqualTo(1));
	}

	[Test]
	public void Register_moves_target_between_fields()
	{
		var source = new TestErrorsSource();
		source.SetErrorsWithoutNotification("OldName", "Old required");
		source.SetErrorsWithoutNotification("NewName", "New required");
		var target = new TestTarget();

		using var controller = CreateController();
		controller.SetErrorsSource(source);
		controller.Register("OldName", target);
		controller.Register("NewName", target);

		source.RaiseErrorsChanged("OldName");
		Assert.That(target.ErrorText, Is.EqualTo("New required"));

		source.SetErrors("NewName");
		Assert.That(target.HasError, Is.False);
		Assert.That(target.ErrorText, Is.Null);
	}

	private static NotifyDataErrorValidationController<TestTarget> CreateController() =>
		new((target, hasError, errorText) =>
		{
			target.HasError = hasError;
			target.ErrorText = errorText;
		});

	private sealed class TestTarget
	{
		public bool HasError { get; set; }

		public string? ErrorText { get; set; }
	}

	private sealed class TestErrorsSource : INotifyDataErrorInfo
	{
		private readonly Dictionary<string, List<string>> _errors = new(StringComparer.Ordinal);
		private EventHandler<DataErrorsChangedEventArgs>? _errorsChanged;

		public int RemoveCount { get; private set; }

		public bool HasErrors => _errors.Count > 0;

		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged
		{
			add => _errorsChanged += value;
			remove
			{
				RemoveCount++;
				_errorsChanged -= value;
			}
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			return !string.IsNullOrEmpty(propertyName) && _errors.TryGetValue(propertyName, out var errors)
				? errors
				: Enumerable.Empty<string>();
		}

		public void SetErrors(string propertyName, params string[] errors)
		{
			SetErrorsWithoutNotification(propertyName, errors);
			RaiseErrorsChanged(propertyName);
		}

		public void SetErrorsWithoutNotification(string propertyName, params string[] errors)
		{
			if (errors.Length == 0)
				_errors.Remove(propertyName);
			else
				_errors[propertyName] = errors.ToList();
		}

		public void RaiseErrorsChanged(string propertyName)
		{
			_errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}
	}
}
