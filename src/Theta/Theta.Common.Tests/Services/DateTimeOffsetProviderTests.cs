using Theta.Common.Services;

namespace Theta.Common.Tests.Services;

public class DateTimeOffsetProviderTests
{
    // Now

    [Fact]
    public void Now_ShouldReturnConfiguredDateTime_WhenConfiguredInContext()
    {
        var date = new DateTimeOffset(2022, 3, 1, 13, 14, 15, TimeSpan.FromHours(-4));
        using var dateContext = new DateTimeOffsetProviderContext(date);

        DateTimeOffsetProvider.Now.Should().Be(date);
    }
    
    [Fact]
    public void Now_ShouldReturnCurrentDateTime_WhenNotConfiguredInContext()
    {
        DateTimeOffsetProvider.Now.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
    }
}