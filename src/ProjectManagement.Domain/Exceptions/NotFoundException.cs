namespace ProjectManagement.Domain.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string resourceName, object resourceKey)
        : base($"{resourceName} with identifier '{resourceKey}' was not found.")
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }

    public string ResourceName { get; }

    public object ResourceKey { get; }
}
