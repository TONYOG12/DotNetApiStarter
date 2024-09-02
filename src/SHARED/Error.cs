namespace SHARED;

public record Error
{
    protected Error(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        Type = errorType;
    }
    
    public string Code { get; set; }
    public string Description { get; set; }
    public ErrorType Type { get; set; }

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.", ErrorType.Validation);

    public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.", ErrorType.Validation);
    
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
    
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict,
    Failure
}