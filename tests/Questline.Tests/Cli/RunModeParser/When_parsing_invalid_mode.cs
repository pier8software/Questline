using Questline.Cli;

namespace Questline.Tests.Cli;

public class When_parsing_invalid_mode
{
    [Fact]
    public void Throws_for_invalid_mode()
    {
        var exception = Should.Throw<ArgumentException>(() => RunModeParser.Parse(["--mode=invalid"]));

        exception.Message.ShouldContain("Unknown mode 'invalid'");
        exception.Message.ShouldContain("game");
        exception.Message.ShouldContain("deploy-content");
    }
}
