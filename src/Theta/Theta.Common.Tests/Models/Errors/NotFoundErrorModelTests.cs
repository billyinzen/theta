using Theta.Common.Exceptions;
using Theta.Common.Helpers;
using Theta.Common.Models.Errors;

namespace Theta.Common.Tests.Models.Errors;

public class NotFoundErrorModelTests
{
    public NotFoundErrorModelTests()
    {
        DateTimeOffsetHelper.Reset();
    }
    
    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);

        var exception = new NotFoundException(typeof(NotFoundException), Guid.NewGuid());
        var actual = NotFoundErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(NotFoundErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.ResourceType.Should().BeEquivalentTo(nameof(NotFoundException));
        actual.Id.Should().Be(exception.Id);
    } 
}