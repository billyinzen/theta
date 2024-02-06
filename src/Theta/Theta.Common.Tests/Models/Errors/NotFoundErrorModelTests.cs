using Theta.Common.Exceptions;
using Theta.Common.Models.Errors;
using Theta.Common.Services;

namespace Theta.Common.Tests.Models.Errors;

public class NotFoundErrorModelTests
{
    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        using var dateContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);

        var exception = new NotFoundException(typeof(NotFoundException), Guid.NewGuid());
        var actual = NotFoundErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(NotFoundErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.ResourceType.Should().BeEquivalentTo(nameof(NotFoundException));
        actual.Id.Should().Be(exception.Id);
    } 
}