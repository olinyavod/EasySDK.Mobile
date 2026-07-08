using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EasySDK.Mobile.ViewModels.Validation;

public sealed class NotifyDataErrorValidationController<TTarget> : IDisposable
	where TTarget : class
{
	private readonly Dictionary<string, List<TTarget>> _targetsByFieldName = new(StringComparer.Ordinal);
	private readonly Action<TTarget, bool, string?>    _applyError;

	private INotifyDataErrorInfo? _errorsSource;
	private bool                  _disposed;

	public NotifyDataErrorValidationController(Action<TTarget, bool, string?> applyError)
	{
		_applyError = applyError ?? throw new ArgumentNullException(nameof(applyError));
	}

	public void SetErrorsSource(object? source)
	{
		ThrowIfDisposed();

		var errorsSource = source as INotifyDataErrorInfo;
		if (ReferenceEquals(_errorsSource, errorsSource))
			return;

		DetachErrorsSource();
		_errorsSource = errorsSource;

		if (_errorsSource is not null)
			_errorsSource.ErrorsChanged += OnErrorsChanged;

		ApplyAllErrors();
	}

	public void Register(string? fieldName, TTarget target)
	{
		ThrowIfDisposed();
		if (target is null)
			throw new ArgumentNullException(nameof(target));

		Unregister(target, clearError: false);

		if (string.IsNullOrWhiteSpace(fieldName))
		{
			_applyError(target, false, null);
			return;
		}

		if (!_targetsByFieldName.TryGetValue(fieldName, out var targets))
		{
			targets = new List<TTarget>();
			_targetsByFieldName[fieldName] = targets;
		}

		targets.Add(target);
		ApplyErrors(fieldName);
	}

	public void Unregister(TTarget target, bool clearError = true)
	{
		ThrowIfDisposed();
		if (target is null)
			throw new ArgumentNullException(nameof(target));

		foreach (var (fieldName, targets) in _targetsByFieldName.ToArray())
		{
			targets.RemoveAll(i => ReferenceEquals(i, target));

			if (targets.Count == 0)
				_targetsByFieldName.Remove(fieldName);
		}

		if (clearError)
			_applyError(target, false, null);
	}

	public void ClearTargets(bool clearErrors = true)
	{
		ThrowIfDisposed();

		if (clearErrors)
		{
			foreach (var target in _targetsByFieldName.Values.SelectMany(i => i).Distinct(ReferenceEqualityComparer<TTarget>.Instance))
				_applyError(target, false, null);
		}

		_targetsByFieldName.Clear();
	}

	public void ApplyErrors(string? fieldName)
	{
		ThrowIfDisposed();

		if (string.IsNullOrEmpty(fieldName))
		{
			ApplyAllErrors();
			return;
		}

		if (!_targetsByFieldName.TryGetValue(fieldName, out var targets))
			return;

		var errorText = GetFirstError(fieldName);
		var hasError  = !string.IsNullOrWhiteSpace(errorText);

		foreach (var target in targets)
			_applyError(target, hasError, errorText);
	}

	public void ApplyAllErrors()
	{
		ThrowIfDisposed();

		foreach (var fieldName in _targetsByFieldName.Keys.ToArray())
			ApplyErrors(fieldName);
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;
		DetachErrorsSource();
		_targetsByFieldName.Clear();
	}

	private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
	{
		ApplyErrors(e.PropertyName);
	}

	private string? GetFirstError(string fieldName)
	{
		if (_errorsSource is null)
			return null;

		return _errorsSource.GetErrors(fieldName)
			?.OfType<object?>()
			.Select(i => i?.ToString())
			.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i));
	}

	private void DetachErrorsSource()
	{
		if (_errorsSource is not null)
			_errorsSource.ErrorsChanged -= OnErrorsChanged;

		_errorsSource = null;
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
			throw new ObjectDisposedException(GetType().FullName);
	}

	private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
		where T : class
	{
		public static readonly ReferenceEqualityComparer<T> Instance = new();

		public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

		public int GetHashCode(T obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
	}
}
