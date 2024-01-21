using Theta.Common.Helpers;
using Theta.Common.Models.Errors;

namespace Theta.Common.Tests.Models.Errors;

public class ApplicationErrorModelTests
{
    public ApplicationErrorModelTests()
    {
        DateTimeOffsetHelper.Reset();
    }
    
    // FromException

    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);

        var exception = new ArgumentNullException(nameof(Exception));
        var actual = ApplicationErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(ApplicationErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.ErrorType.Should().BeEquivalentTo(nameof(ArgumentNullException));
        actual.Exception.Should().BeEquivalentTo(exception.Message);
    } 
}