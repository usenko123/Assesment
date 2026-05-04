namespace Assessment.Domain.Common;

public readonly struct Result<T>
{
    private readonly T? _value;
    private readonly AppError? _error;

    private Result(T? value, AppError? error)
    {
        _value = value;
        _error = error;
        IsSuccess = error is null;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Result has no value.");

    public AppError Error =>
        IsFailure ? _error! : throw new InvalidOperationException("Result has no error.");

    public static Result<T> Ok(T value) => new(value, null);

    public static Result<T> Fail(AppError error) => new(default, error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<AppError, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);
}
