using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasySDK.Mobile.ViewModels.Attributes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EasySDK.Mobile.ViewModels;

public abstract class DataViewModelBase : ViewModelBase, INotifyDataErrorInfo
{
	#region Private fields

	private readonly IValidator _validator;
	private readonly Dictionary<string, ICollection<string>> _errors = new();

	private bool _hasErrors;

	#endregion

	#region Events

	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	#endregion

	#region Properties

	public bool HasErrors
	{
		get => _hasErrors;
		set => SetProperty(ref _hasErrors, value);
	}

	#endregion

	#region ctor

	protected DataViewModelBase
	(
		IServiceScopeFactory scopeFactory,
		IValidator validator
	):base(scopeFactory)
	{
		_validator = validator ?? throw new ArgumentNullException(nameof(validator));
	}

	#endregion

	#region Public methods

	public IEnumerable GetErrors(string propertyName)
	{
		return _errors.TryGetValue(propertyName, out var errors) 
			? errors
			: Enumerable.Empty<object>();
	}

	#endregion

	#region Protected methods

	protected void ClearErrors()
	{
		var errors = _errors.Keys.ToArray();
		_errors.Clear();

		HasErrors = false;

		foreach (var error in errors)
			RaiseErrorsChanged(error);
	}

	protected void ClearErrors(string propertyName)
	{
		if(!_errors.Remove(propertyName))
			return;

		HasErrors = _errors.Count > 0;
		RaiseErrorsChanged(propertyName);
	}

	protected void SetErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = "", bool raiseErrorsChanged = true)
	{
		_errors[propertyName] = errors.ToList();

		if (raiseErrorsChanged)
			RaiseErrorsChanged(propertyName);
	}

	protected void SetError(string errorMessage, [CallerMemberName] string propertyName = "", bool raiseErrorsChanged = true)
	{
		if (!_errors.TryGetValue(propertyName, out var errors))
		{
			errors = new List<string>();
			_errors[propertyName] = errors;
		}

		errors.Add(errorMessage);

		if (raiseErrorsChanged)
			RaiseErrorsChanged(propertyName);
	}

	public async Task<bool> ValidateAsync()
	{
		ClearErrors();

		var result = await _validator.ValidateAsync(new ValidationContext<object>(this));

		foreach (var failure in result.Errors)
		{
			var message = string.Format(failure.ErrorMessage, failure.FormattedMessagePlaceholderValues.Values.ToArray());
			SetError(message, failure.PropertyName, false);
		}

		foreach (var error in _errors)
			RaiseErrorsChanged(error.Key);

		HasErrors = !result.IsValid;

		return result.IsValid;
	}

	protected void RaiseErrorsChanged(string propertyName)
	{
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}

	protected override void OnPropertyChanged(string propertyName = "")
	{
		base.OnPropertyChanged(propertyName);
		ClearErrors(propertyName);
	}

	protected void SetDataErrors(IDictionary<string, IEnumerable<string>>? errors)
	{
		if(errors == null || errors.Count == 0)
			return;

		var propertiesMap = TypeDescriptor.GetProperties(this)
			.OfType<PropertyDescriptor>()
			.ToDictionary(GetPropertyName, i => i.Name);

		foreach (var error in errors)
		{
			if(!propertiesMap.TryGetValue(error.Key, out var propertyName))
				continue;

			SetErrors(error.Value, propertyName);
		}
	}

	#endregion

	#region Private methods

	private string GetPropertyName(PropertyDescriptor propertyDescriptor)
	{
		if (propertyDescriptor.Attributes[typeof(FieldNameAttribute)] is FieldNameAttribute att)
			return att.Name ?? propertyDescriptor.Name;

		return propertyDescriptor.Name;
	}

	#endregion
}