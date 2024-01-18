using FluentValidation;

namespace Theta.Api.Errors;

/// <summary>
/// Read model representing a validation failure
/// </summary>
public class ValidationErrorModel : BaseErrorModel
{
    internal const string ErrorMessage = "Validation Error";
    
    /// <summary>
    /// Collection of validation errors
    /// </summary>
    public Dictionary<string, IEnumerable<string>> Errors { get; }

    /// <summary>
    /// Initialize a new instance of the <see cref="ValidationErrorModel"/> class
    /// </summary>
    /// <param name="errors"></param>
    private ValidationErrorModel(Dictionary<string, IEnumerable<string>> errors)
        : base(ErrorMessage)
    {
        Errors = errors;
    }

    /// <summary>
    /// Create a new <see cref="ValidationErrorModel"/> from a <see cref="ValidationException"/>
    /// </summary>
    /// <param name="exception"></param>
    public static ValidationErrorModel FromException(ValidationException exception)
        => new(exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    keySelector: group => group.Key,
                    elementSelector: group => group.Select(error => error.ErrorMessage)));
}