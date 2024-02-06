using Theta.Common.Models.Errors;
using Theta.Common.Services;

namespace Theta.Common.Tests.Models.Errors;

public class ApplicationErrorModelTests
{
    // FromException

    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        using var dateContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);

        var exception = new ArgumentNullException(nameof(Exception));
        var actual = ApplicationErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(ApplicationErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.ErrorType.Should().BeEquivalentTo(nameof(ArgumentNullException));
        actual.Exception.Should().BeEquivalentTo(exception.Message);
    } 
}