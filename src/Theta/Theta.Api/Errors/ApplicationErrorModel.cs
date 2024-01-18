namespace Theta.Api.Errors;

/// <summary>
/// Read model representing generic application errors  
/// </summary>
public class ApplicationErrorModel : BaseErrorModel
{
    internal const string ErrorMessage = "Unexpected Error";
    
    /// <summary>
    /// The type of internal error
    /// </summary>
    public string ErrorType { get; private init; }
    
    /// <summary>
    /// Description of the internal error
    /// </summary>
    public string Exception { get; private init; }

    /// <summary>
    /// Initialize a new instance of the <see cref="ApplicationErrorModel"/> class
    /// </summary>
    /// <param name="errorType"></param>
    /// <param name="exception"></param>
    private ApplicationErrorModel(string errorType, string exception)
        : base(ErrorMessage)
    {
        ErrorType = errorType;
        Exception = exception;
    }

    /// <summary>
    /// Create a new <see cref="ApplicationErrorModel"/> from an <see cref="Exception"/>
    /// </summary>
    /// <param name="exception">The exception from which to generate the error model</param>
    public static ApplicationErrorModel FromException(Exception exception)
        => new (exception.GetType().Name, exception.Message);
}