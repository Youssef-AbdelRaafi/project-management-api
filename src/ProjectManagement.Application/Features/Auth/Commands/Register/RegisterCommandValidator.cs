using FluentValidation;
using ProjectManagement.Domain.Common;

namespace ProjectManagement.Application.Features.Auth.Commands.Register;

/// <summary>
/// Validates register requests before the handler creates an identity user.
/// </summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const int EmailMaxLength = 256;
    private const int PasswordMinLength = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCommandValidator" /> class.
    /// </summary>
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(EmailMaxLength);

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(PasswordMinLength)
            .Must(password => password.Any(char.IsUpper))
            .WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => password.Any(char.IsDigit))
            .WithMessage("Password must contain at least one digit.");

        RuleFor(command => command.FullName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.FullNameMaxLength);
    }
}
