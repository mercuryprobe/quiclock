using Quiclock.Services;

namespace Quiclock.Tests;

public sealed class DurationParserTests
{
    [Theory]
    [InlineData("5", 300)]
    [InlineData("1.5", 90)]
    [InlineData("90s", 90)]
    [InlineData("2:30", 150)]
    public void TryParse_AcceptsSupportedFormats(string input, int expectedSeconds)
    {
        var success = DurationParser.TryParse(input, out var duration, out var error);

        Assert.True(success);
        Assert.Equal(string.Empty, error);
        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), duration);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("1:99")]
    [InlineData("abc")]
    public void TryParse_RejectsInvalidInputs(string input)
    {
        var success = DurationParser.TryParse(input, out _, out var error);

        Assert.False(success);
        Assert.NotEqual(string.Empty, error);
    }
}
