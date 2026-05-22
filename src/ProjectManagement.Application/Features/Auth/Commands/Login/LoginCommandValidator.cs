using FluentValidation;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private const int EmailMaxLength = 256;

    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(EmailMaxLength);

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
