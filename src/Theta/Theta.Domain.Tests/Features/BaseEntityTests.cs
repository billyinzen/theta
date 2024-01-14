using FluentAssertions;
using Theta.Domain.Features.Venues;

namespace Theta.Domain.Tests.Features;

public class BaseEntityTests
{
    // EntityTag
    
    [Fact]
    public void EntityTag_ShouldGenerateEtag()
    {
        var guid = new Guid("855764ff-3dff-428d-bb29-a3467543c979");
        var created = DateTimeOffset.MinValue;
        var modified = DateTimeOffset.UnixEpoch;
        const string expected = "\"04CC48CF02CC41195164B3B36BDEC8BD\"";
        
        var entity = new Venue("Name")
        {
            Id = guid,
            CreatedDate = created,
            ModifiedDate = modified
        };

        entity.EntityTag.Should().BeEquivalentTo(expected);
    }
    
    // CompareEtag
    
    [Fact]
    public void CompareEtag_ShouldReturnTrue_WhenEtagMatches()
    {
        var guid = new Guid("855764ff-3dff-428d-bb29-a3467543c979");
        var created = DateTimeOffset.MinValue;
        var modified = DateTimeOffset.UnixEpoch;
        const string expected = "\"04CC48CF02CC41195164B3B36BDEC8BD\"";
        
        var entity = new Venue("Name")
        {
            Id = guid,
            CreatedDate = created,
            ModifiedDate = modified
        };

        entity.CompareEtag(expected).Should().BeTrue();
    }
    
    [Fact]
    public void CompareEtag_ShouldReturnFalse_WhenEtagDoesNotMatch()
    {
        var guid = new Guid("855764ff-3dff-428d-bb29-a3467543c979");
        var created = DateTimeOffset.MinValue;
        var modified = DateTimeOffset.UnixEpoch.AddSeconds(1);
        const string expected = "\"04CC48CF02CC41195164B3B36BDEC8BD\"";
        
        var entity = new Venue("Name")
        {
            Id = guid,
            CreatedDate = created,
            ModifiedDate = modified
        };

        entity.CompareEtag(expected).Should().BeFalse();
    }
    
    [Fact]
    public void CompareEtag_ShouldReturnFalse_WhenProvidedEtagEmpty()
    {
        var guid = new Guid("855764ff-3dff-428d-bb29-a3467543c979");
        var created = DateTimeOffset.MinValue;
        var modified = DateTimeOffset.UnixEpoch.AddSeconds(1);
        const string expected = "";
        
        var entity = new Venue("Name")
        {
            Id = guid,
            CreatedDate = created,
            ModifiedDate = modified
        };

        entity.CompareEtag(string.Empty).Should().BeFalse();
    }
}