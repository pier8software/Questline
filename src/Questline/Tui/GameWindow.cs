using Terminal.Gui;

namespace Questline.Tui;

public class GameWindow : Window
{
    private readonly CommandParser _parser;
    private readonly CommandHandler _handler;
    private readonly TextView _outputView;
    private readonly TextField _inputField;
    private GameState _state;

    public GameWindow(CommandParser parser, CommandHandler handler)
    {
        _parser = parser;
        _handler = handler;
        _state = new GameState();

        Title = $"Questline ({Application.QuitKey} to quit)";

        var outputFrame = new FrameView
        {
            Title = "Adventure",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(3)
        };

        _outputView = new TextView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true
        };

        outputFrame.Add(_outputView);

        var inputFrame = new FrameView
        {
            Title = "Command",
            X = 0,
            Y = Pos.AnchorEnd(3),
            Width = Dim.Fill(),
            Height = 3
        };

        _inputField = new TextField
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 1
        };

        _inputField.Accepting += OnCommandSubmit;

        inputFrame.Add(_inputField);

        Add(outputFrame, inputFrame);

        ShowWelcome();
    }

    private void ShowWelcome()
    {
        AppendOutput("Welcome to Questline!");
        AppendOutput("");
        AppendOutput(_handler.DescribeRoom(_state));
        AppendOutput("");
    }

    private void OnCommandSubmit(object? sender, CommandEventArgs e)
    {
        var input = _inputField.Text;
        _inputField.Text = "";

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        AppendOutput($"> {input}");

        var command = _parser.Parse(input);
        var result = _handler.Execute(command, _state);

        AppendOutput(result.Output);
        AppendOutput("");

        _state = result.NewState;

        if (_state.Flags.GetValueOrDefault("quit"))
        {
            Application.RequestStop();
        }
    }

    private void AppendOutput(string text)
    {
        if (_outputView.Text.Length > 0)
        {
            _outputView.Text += Environment.NewLine;
        }
        _outputView.Text += text;

        _outputView.MoveEnd();
    }
}
