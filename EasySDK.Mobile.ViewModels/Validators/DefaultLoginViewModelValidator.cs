using EasySDK.Mobile.Models;
using FluentValidation;

namespace EasySDK.Mobile.ViewModels.Validators;

public class DefaultLoginViewModelValidator : AbstractValidator<ILoginForm>
{
	public DefaultLoginViewModelValidator()
	{
		RuleFor(m => m.Login)
			.NotEmpty()
			.WithName(Properties.Resources.Login);

		RuleFor(m => m.Password)
			.NotEmpty()
			.WithName(Properties.Resources.Password);
	}
}