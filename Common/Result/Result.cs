using System.Diagnostics.CodeAnalysis;

namespace Common.Result;
public class Result
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public List<Result> Results { get; } = [];

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public void AddResult(Result result)
    {
        IsSuccess = result.IsSuccess;
        Message = result.Message;
        Results.Add(result);
    }
    
    public static Result Fail(string message)
    {
        return new Result(false, message);
    }

    public static Result<T> Fail<T>(string message)
    {
        return new Result<T>(default(T), false, message);
    }
    public static Result Ok()
    {
        return new Result(true, string.Empty);
    }
    public static Result Ok(string message)
    {
        return new Result(true, message);
    }
    public static Result<T> Ok<T>(T value,string message)
    {
        return new Result<T>(value, true, message);
    }
    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (Result result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Ok();
    }
}
public class Result<T> : Result
{
    private readonly T _value;

    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException();

            return _value;
        }
    }

     public Result([AllowNull] T value, bool isSuccess, string message)
        : base(isSuccess, message)
    {
        _value = value;
    }
}