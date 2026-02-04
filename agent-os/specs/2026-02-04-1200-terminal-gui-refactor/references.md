# References: Terminal.Gui

## Official Documentation

- GitHub: https://github.com/gui-cs/Terminal.Gui
- API Documentation: https://gui-cs.github.io/Terminal.Gui/

## Key Classes Used

### Window
Base class for application windows. Contains other views.

### FrameView
A view with a border and title. Used for grouping related content.

### TextView
Multi-line text display. Supports scrolling. Used for game output.

### TextField
Single-line text input. Used for command entry.

### Application
Static class managing the application lifecycle:
- `Application.Init()` - Initialize the library
- `Application.Run(Toplevel)` - Start the main loop
- `Application.Shutdown()` - Clean up resources
- `Application.RequestStop()` - Exit the main loop

## Layout System (v2)

Terminal.Gui v2 uses a Pos/Dim system:
- `Pos.At(n)` - Absolute position
- `Pos.Percent(n)` - Percentage position
- `Dim.Fill()` - Fill remaining space
- `Dim.Auto()` - Size to content
- `Dim.Percent(n)` - Percentage of parent

## Event Handling

- `KeyDown` event for keyboard input
- `Accept` event for TextField submission (Enter key)
