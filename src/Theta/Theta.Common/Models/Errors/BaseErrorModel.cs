using Theta.Common.Helpers;
using Theta.Common.Services;

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
    public DateTimeOffset Timestamp = DateTimeOffsetProvider.Now;
    
    /// <summary>
    /// Initialize a new instance of the <see cref="BaseErrorModel"/> class
    /// </summary>
    /// <param name="message"></param>
    protected BaseErrorModel(string message)
    {
        Message = message;
    }
}