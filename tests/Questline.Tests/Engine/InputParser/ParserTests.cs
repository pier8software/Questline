using Questline.Engine.InputParsers;
using Questline.Framework.Mediator;

namespace Questline.Tests.Engine;

public class ParserTests
{
    private readonly Parser _parser;

    public ParserTests()
    {
        _parser = new ParserBuilder()
            .RegisterCommand<VerbNoArgsRequest>(["verb", "v"], _ => new VerbNoArgsRequest())
            .RegisterCommand<VerbWithArgsRequest>(["args"], args => new VerbWithArgsRequest(args))
            .Build();
    }

    [Fact]
    public void Single_word_returns_verb_with_no_args()
    {
        var result = _parser.Parse("verb");

        result.AsT0.ShouldBeOfType<VerbNoArgsRequest>();
    }

    [Fact]
    public void Alias_parsed_to_the_same_command()
    {
        var result = _parser.Parse("v");

        result.AsT0.ShouldBeOfType<VerbNoArgsRequest>();
    }

    [Fact]
    public void Verb_with_argument_splits_correctly()
    {
        var result = _parser.Parse("args example");

        result.IsT0.ShouldBeTrue();
        var command = result.AsT0 as VerbWithArgsRequest;
        command!.Args.ShouldBe(["example"]);
    }

    [Fact]
    public void Input_is_converted_to_lowercase()
    {
        var result = _parser.Parse("ARGS Example");

        result.IsT0.ShouldBeTrue();
        var command = result.AsT0 as VerbWithArgsRequest;
        command!.Args.ShouldBe(["example"]);
    }

    [Fact]
    public void Leading_and_trailing_whitespace_is_trimmed()
    {
        var result = _parser.Parse("  verb  ");

        result.AsT0.ShouldBeOfType<VerbNoArgsRequest>();
    }

    [Fact]
    public void Extra_whitespace_between_words_is_ignored()
    {
        var result = _parser.Parse("args   example");

        result.IsT0.ShouldBeTrue();
        var command = result.AsT0 as VerbWithArgsRequest;
        command!.Args.ShouldBe(["example"]);
    }

    [Fact]
    public void Empty_input_returns_parse_error()
    {
        var result = _parser.Parse("");

        result.AsT1.ShouldBeOfType<ParseError>();
        result.AsT1.Message.ShouldBe("Please enter a command.");
    }

    [Fact]
    public void Unknown_verb_returns_error()
    {
        var result = _parser.Parse("error");

        result.AsT1.ShouldBeOfType<ParseError>();
        result.AsT1.Message.ShouldBe("I don't understand 'error'. Type 'help' for available commands.");
    }
}

public record VerbNoArgsRequest : IRequest;

public record VerbWithArgsRequest(string[] Args) : IRequest;
