namespace Questline.Tests.Cli.RunModeParser;

public class When_parsing_invalid_mode
{
    [Fact]
    public void Throws_for_invalid_mode()
    {
        var exception = Should.Throw<ArgumentException>(() => Questline.Cli.RunModeParser.Parse(["--mode=invalid"]));

        exception.Message.ShouldContain("Unknown mode 'invalid'");
        exception.Message.ShouldContain("game");
        exception.Message.ShouldContain("deploy-content");
    }
}
