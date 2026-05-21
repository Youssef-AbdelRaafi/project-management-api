using System.Reflection;

namespace ProjectManagement.Application.Common.Behaviors;

internal static class RequestLogSanitizer
{
    private const string MaskedValue = "***";

    private static readonly string[] SensitivePropertyNameParts =
    [
        "password",
        "token",
        "secret",
        "credential"
    ];

    public static IReadOnlyDictionary<string, object?> Sanitize<TRequest>(TRequest request)
        where TRequest : notnull
    {
        var properties = typeof(TRequest)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0);

        return properties.ToDictionary(
            property => property.Name,
            property => IsSensitive(property.Name) ? MaskedValue : property.GetValue(request));
    }

    private static bool IsSensitive(string propertyName)
    {
        return SensitivePropertyNameParts.Any(part =>
            propertyName.Contains(part, StringComparison.OrdinalIgnoreCase));
    }
}
