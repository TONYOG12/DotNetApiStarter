namespace SHARED;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error != Error.None:
                throw new InvalidOperationException();
            case false when error == Error.None:
                throw new InvalidOperationException();
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }
    
    protected Result(bool isSuccess, IEnumerable<Error> errors)
    {
        var errorList = errors.ToList();
        
        switch (isSuccess)
        {
            case true when errorList.Count != 0:
                throw new InvalidOperationException("Success result cannot contain errors.");
            case false when errorList.Count == 0:
                throw new InvalidOperationException("Failure result must contain at least one error.");
            default:
                IsSuccess = isSuccess;
                Errors = errorList;
                break;
        }
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }
    public List<Error> Errors { get; } = [];
    
    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);
    
    public static Result Failure(List<Error> errors) => new(false, errors);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    
    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);

    public static Result Create(bool condition) => condition ? Success() : Failure(Error.ConditionNotMet);
    
    public static implicit operator Result(Error error) => Failure(error);
    public static implicit operator Result(List<Error> error) => Failure(error);

    protected static Result<TValue> Create<TValue>(TValue value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    protected internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;
    
    protected internal Result(TValue value, bool isSuccess, List<Error> error)
        : base(isSuccess, error) =>
        _value = value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue value) => Create(value);
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
    
    public static implicit operator Result<TValue>(List<Error> error) => Failure<TValue>(error);
}