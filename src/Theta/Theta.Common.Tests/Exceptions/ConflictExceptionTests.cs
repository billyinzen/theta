using FluentAssertions;
using Theta.Common.Exceptions;

namespace Theta.Common.Tests.Exceptions;

public class ConflictExceptionTests
{
    [Fact]
    public void Ctor_InitializesExceptionWithGivenParameters()
    {
        var type = typeof(ConflictExceptionTests);
        var guid = Guid.NewGuid();
        const string entityTag = "actual entity tag";
        const string providedEntityTag = "provided entity tag";

        var exception = new ConflictException(type, guid, entityTag, providedEntityTag);
        
        exception.Message.Should().BeEquivalentTo(ConflictException.ErrorMessage);
        exception.Type.Should().Be(type);
        exception.Id.Should().Be(guid);
        exception.EntityTag.Should().BeEquivalentTo(entityTag);
        exception.ProvidedEntityTag.Should().BeEquivalentTo(providedEntityTag);
    }
}