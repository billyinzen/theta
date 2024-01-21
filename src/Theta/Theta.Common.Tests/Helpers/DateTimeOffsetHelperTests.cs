using Theta.Common.Helpers;

namespace Theta.Common.Tests.Helpers;

public class DateTimeOffsetHelperTests
{
    [Fact]
    public void Now_ReturnsConfiguredTimestamp_WhenTimestampSet()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        var actual = DateTimeOffsetHelper.Now();
        actual.Should().Be(DateTimeOffset.UnixEpoch);
    }
    
    [Fact]
    public void Now_ReturnsCurrentTimestamp_WhenTimestampReset()
    {
        DateTimeOffsetHelper.Set(DateTimeOffset.UnixEpoch);
        DateTimeOffsetHelper.Reset();
        var actual = DateTimeOffsetHelper.Now();
        
        actual.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
    }
}