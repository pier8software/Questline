using Questline.Cli;

namespace Questline.Tests.Cli;

public class RunModeParserTests
{
    [Fact]
    public void Defaults_to_game_when_no_args()
    {
        var mode = RunModeParser.Parse([]);

        mode.ShouldBe(RunMode.Game);
    }

    [Fact]
    public void Returns_game_for_explicit_game_mode()
    {
        var mode = RunModeParser.Parse(["--mode=game"]);

        mode.ShouldBe(RunMode.Game);
    }

    [Fact]
    public void Returns_deploy_content_for_deploy_content_mode()
    {
        var mode = RunModeParser.Parse(["--mode=deploy-content"]);

        mode.ShouldBe(RunMode.DeployContent);
    }

    [Fact]
    public void Is_case_insensitive()
    {
        var mode = RunModeParser.Parse(["--mode=Deploy-Content"]);

        mode.ShouldBe(RunMode.DeployContent);
    }

    [Fact]
    public void Throws_for_invalid_mode()
    {
        var exception = Should.Throw<ArgumentException>(() => RunModeParser.Parse(["--mode=invalid"]));

        exception.Message.ShouldContain("Unknown mode 'invalid'");
        exception.Message.ShouldContain("game");
        exception.Message.ShouldContain("deploy-content");
    }
}
