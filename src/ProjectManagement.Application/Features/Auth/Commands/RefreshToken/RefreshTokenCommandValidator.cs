using FluentValidation;

namespace ProjectManagement.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Validates refresh token rotation requests.
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenCommandValidator" /> class.
    /// </summary>
    public RefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty();
    }
}
