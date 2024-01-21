using Theta.Common.Models;

namespace Theta.Common.Tests.Models;

public class CommandResultTests
{
    
    // Implicit cast from bool

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ImplicitCast_ShouldMapIsSuccess_FromBoolean(bool input)
    {
        var sut = (CommandResult)input;
        sut.IsSuccess.Should().Be(input);
        sut.Error.Should().BeNull();
    }
    
    // Implicit cast from string

    [Fact]
    public void ImplicitCast_ShouldIndicateError()
    {
        const string errorMessage = "test error";
        var sut = (CommandResult)errorMessage;
        sut.IsSuccess.Should().BeFalse();
        sut.Error.Should().BeEquivalentTo(errorMessage);
    }
}