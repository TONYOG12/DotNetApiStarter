using SHARED;

namespace APP.Services.Storage;

public static class StorageErrors
{
    public static Error PortNotFound(string name) =>
        Error.NotFound("NotFound", $"The port {name} was not found");
    
    public static Error SaveFileFailure(string message) =>
        Error.Failure("Failure", $"Failed to save the file. Message: {message}");
}