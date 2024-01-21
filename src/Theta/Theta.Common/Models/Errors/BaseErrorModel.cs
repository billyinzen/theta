using Theta.Common.Helpers;

namespace Theta.Common.Models.Errors;

/// <summary>
/// Base class for error models
/// </summary>
public abstract class BaseErrorModel
{
    /// <summary>
    /// Description of the error that is being represented
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Timestamp of the error
    /// </summary>
    public DateTimeOffset Timestamp = DateTimeOffsetHelper.Now();
    
    /// <summary>
    /// Initialize a new instance of the <see cref="BaseErrorModel"/> class
    /// </summary>
    /// <param name="message"></param>
    protected BaseErrorModel(string message)
    {
        Message = message;
    }
}