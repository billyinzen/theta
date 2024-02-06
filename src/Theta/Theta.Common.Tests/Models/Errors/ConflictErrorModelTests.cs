using Theta.Common.Exceptions;
using Theta.Common.Models.Errors;
using Theta.Common.Services;

namespace Theta.Common.Tests.Models.Errors;

public class ConflictErrorModelTests
{
    [Fact]
    public void FromException_ShouldGenerateModel_FromException()
    {
        using var dateContext = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        
        var exception = new ConflictException(typeof(ConflictException), Guid.NewGuid(), "actual", "provided");
        var actual = ConflictErrorModel.FromException(exception);

        actual.Message.Should().BeEquivalentTo(ConflictErrorModel.ErrorMessage);
        actual.Timestamp.Should().Be(DateTimeOffset.UnixEpoch);
        actual.ResourceType.Should().BeEquivalentTo(nameof(ConflictException));
        actual.Id.Should().Be(exception.Id);
        actual.Current.Should().BeEquivalentTo(exception.EntityTag);
        actual.Requested.Should().BeEquivalentTo(exception.ProvidedEntityTag);
    } 
}