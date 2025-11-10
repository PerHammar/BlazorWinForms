namespace BlazorWinForms.Interop;

/// <summary>
/// Represents the result of a request operation.
/// </summary>
public record Result<T>(bool Success, string? Error, T? Data)
{
    /// <summary>
    /// Creates a successful result with the provided data.
    /// </summary>
    /// <param name="data">The result data.</param>
    /// <returns>A successful result instance.</returns>
    public static Result<T> Ok(T data) => new(true, null, data);

    /// <summary>
    /// Creates a failed result with the provided error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result instance.</returns>
    public static Result<T> Fail(string error) => new(false, error, default);
}
