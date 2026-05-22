namespace ProjectManagement.Application.Common.Models;

public sealed record UserAccount(
    string Id,
    string Email,
    string FullName);
