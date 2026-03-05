using Questline.Cli;

namespace Questline.Tests.Cli.RunModeParser;

public class When_parsing_valid_mode
{
    [Fact]
    public void Defaults_to_game_when_no_args()
    {
        var mode = Questline.Cli.RunModeParser.Parse([]);

        mode.ShouldBe(RunMode.Game);
    }

    [Fact]
    public void Returns_game_for_explicit_game_mode()
    {
        var mode = Questline.Cli.RunModeParser.Parse(["--mode=game"]);

        mode.ShouldBe(RunMode.Game);
    }

    [Fact]
    public void Returns_deploy_content_for_deploy_content_mode()
    {
        var mode = Questline.Cli.RunModeParser.Parse(["--mode=deploy-content"]);

        mode.ShouldBe(RunMode.DeployContent);
    }

    [Fact]
    public void Is_case_insensitive()
    {
        var mode = Questline.Cli.RunModeParser.Parse(["--mode=Deploy-Content"]);

        mode.ShouldBe(RunMode.DeployContent);
    }
}
