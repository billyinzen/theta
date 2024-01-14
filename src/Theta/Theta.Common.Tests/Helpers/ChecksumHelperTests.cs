using FluentAssertions;
using Theta.Common.Helpers;

namespace Theta.Common.Tests.Helpers;

public class ChecksumHelperTests
{
    [Fact]
    public void GetHashValue_GeneratesChecksumForGivenString()
    {
        const string input = "Hello, world!";
        const string expected = "6CD3556DEB0DA54BCA060B4C39479839";
        var hash = ChecksumHelper.GetHashValue(input);
        hash.Should().BeEquivalentTo(expected);
    }
}