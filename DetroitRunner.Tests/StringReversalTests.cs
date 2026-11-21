namespace DetroitRunner.Tests;

public class StringReversalTests
{
    [Theory]
    [InlineData("hello", "olleh")]
    [InlineData("Detroit", "tiorteD")]
    [InlineData("a", "a")]
    [InlineData("", "")]
    public void Reverse_ReturnsCharactersInOppositeOrder(string input, string expected)
    {
        var chars = input.ToCharArray();
        Array.Reverse(chars);
        var actual = new string(chars);

        Assert.Equal(expected, actual);
    }
}
