using FluentValidation;
using FluentValidation.Results;
using Theta.Common.Models.Errors;
using Theta.Common.Services;

namespace Theta.Common.Tests.Models.Errors;

public class ValidationErrorModelTests
{
    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        using var dateContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);

        var validationFailures = new[]
        {
            new ValidationFailure("property_1", "error_1"),
            new ValidationFailure("property_1", "error_2"),
            new ValidationFailure("property_2", "error_3")
        };

        var expected = new Dictionary<string, string[]>
        {
            { "property_1", ["error_1", "error_2"] },
            { "property_2", ["error_3"] }
        };
        
        var exception = new ValidationException(validationFailures);
        var actual = ValidationErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(ValidationErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.Errors.Should().BeEquivalentTo(expected);
    } 
}