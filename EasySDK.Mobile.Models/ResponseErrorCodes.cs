namespace EasySDK.Mobile.Models;

/// <summary>Константы ошибок ответа</summary>
public static class ResponseErrorCodes
{
	/// <summary>Успешный результат (нет ошибок)</summary>
	public const int Ok = 0;

	/// <summary>Неавторизованный доступ к функции сервиса</summary>
	public const int Unauthorized = 401;

	/// <summary>Функция заперещена пользователю</summary>
	public const int NotAccepted = 403;

	/// <summary>
	/// Объект не найден по идентификатору,
	/// если авторизация и по логину/паролю не найден пользователь, то возвращаеться этот код
	/// </summary>
	public const int NotFound = 404;

	/// <summary>Ошибки валидации данных отправленных в запросе</summary>
	public const int DataError = 422;

	/// <summary>Ошибка бизнес логики,
	/// например изменение обхекта, в статусе когда это запрещено</summary>
	public const int BusinessLogicError = 460;
}