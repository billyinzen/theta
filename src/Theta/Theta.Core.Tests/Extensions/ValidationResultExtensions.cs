using FluentValidation.Results;

namespace Theta.Core.Tests.Extensions;

public static class ValidationResultExtensions
{
    public static void ShouldHaveOneError(
        this ValidationResult result, 
        string? propertyName = null, 
        string? errorMessage = null)
    {
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);

        if (propertyName != null)
            result.Errors.First().PropertyName.Should().BeEquivalentTo(propertyName);

        if (errorMessage != null)
            result.Errors.First().ErrorMessage.Should().BeEquivalentTo(errorMessage);
    }
}