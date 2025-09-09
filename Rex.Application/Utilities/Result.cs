using System.Text.Json.Serialization;

namespace Rex.Application.Utilities;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Contains the error information if the operation failed. Ignored during JSON serialization if null.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; set; }

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    protected Result()
    {
        IsSuccess = true;
        Error = default;
    }

    /// <summary>
    /// Initializes a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Allows implicit conversion from <see cref="Error"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) =>
        new(error);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() =>
        new();

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error to include in the result.</param>
    public static Result Failure(Error error) =>
        new(error);
}

/// <summary>
/// Represents the result of an operation that can either succeed with a value of type <typeparamref name="TValue"/> or fail with an error.
/// </summary>
/// <typeparam name="TValue">The type of value returned if the operation is successful.</typeparam>
public class ResultT<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a successful result containing a value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    private ResultT(TValue value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    private ResultT(Error error) : base(error)
    {
        _value = default;
    }

    /// <summary>
    /// Gets the value of the result if successful. Throws <see cref="InvalidOperationException"/> if the result is a failure.
    /// </summary>
    public TValue Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Cannot access Value when IsSuccess is false");

    /// <summary>
    /// Allows implicit conversion from <see cref="Error"/> to <see cref="ResultT{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator ResultT<TValue>(Error error) =>
        new(error);

    /// <summary>
    /// Allows implicit conversion from <typeparamref name="TValue"/> to <see cref="ResultT{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator ResultT<TValue>(TValue value) =>
        new(value);

    /// <summary>
    /// Creates a successful result containing a value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    public static ResultT<TValue> Success(TValue value) =>
        new(value);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error to include in the result.</param>
    public static ResultT<TValue> Failure(Error error) =>
        new(error);
}
