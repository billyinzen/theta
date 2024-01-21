using Theta.Common.Exceptions;

namespace Theta.Common.Tests.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Ctor_InitializesExceptionWithGivenParameters()
    {
        var type = typeof(NotFoundExceptionTests);
        var guid = Guid.NewGuid();

        var exception = new NotFoundException(type, guid);
        
        exception.Message.Should().BeEquivalentTo(NotFoundException.ErrorMessage);
        exception.Type.Should().Be(type);
        exception.Id.Should().Be(guid);
    }
}