using FluentValidation;

namespace ProjectManagement.Application.Features.Auth.Commands.Login;

/// <summary>
/// Validates login requests before credential checks run.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    private const int EmailMaxLength = 256;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandValidator" /> class.
    /// </summary>
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
