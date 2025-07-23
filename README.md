# KameRecorder

KameRecorder is a desktop application that captures user inputs (mouse clicks and key presses) along with desktop screenshots.

## Project Structure

```text
KameRecorder/
├── Abstractions/         → Interfaces for Dependency Injection & testability
├── Models/               → Data models 
├── Services/             → Business logic 
├── Extensions/           → Helper methods
├── Utils/                → ScreenshotCache 
├── Program.cs            → Dependency Injection setup and app entry point
├── MainForm.cs           → WinForms UI (start/stop services)
└── Tests/                → Unit tests
```

## Approach

- Desing with SOLID, OOP, testability in mind

- Thread-Safety:

  - Screen capture operation runs on a dedicated background thread with proper cancellation support
  - use `Channel` to buffer (FIFO) and coordinate input events between the UI hook system and the background processor. This allows for asynchronous processing, avoids race conditions, and ensures thread-safe communication between producers (event hooks) and consumers (event logger).

- Graceful Shutdown: All threads and event handlers cleanly exit on Stop() to avoid exceptions or resource leaks.

- Use .NET 8 LTS to utilize new features and get security update

- Unit Tests are written with `xUnit`, `NSubstitute`, and `Shouldly`. Filesystem interactions are abstracted via `IFileSystem`, allowing true in-memory testing with `MockFileSystem`.

## Features

- The recorder service starts as soon as the `MainForm` is first shown and the service stops when the `MainForm` begins closing

- Captures mouse clicks and key presses using `globalmousekeyhook` library

- Takes desktop screenshots every 200ms (in the background)

- When an event occurs, it:

  - Saves the screenshot (before mouse click / after key press)

  - Saves a line of metadata to a file in CSV format `EventType,Timestamp,InputDetail,MouseLocationX,MouseLocationY,ScreenshotFileName`

- Draws cursor on the screenshot for mouse click events

- Saves files using a globally sortable, readable timestamp format:
`yyyyMMdd_HHmmssfff`

- By default, metadata and screenshots are saved in the `output` and `output\screenshots` directories respectively, located in the same folder as the executable or assembly.

## Potential Improvements

If time permitted or for future iterations, I would consider:

- Multi-project structure for better separation (such as Core, Infrastructure, UI)

- Improved error handling and fallback mechanisms

- Configurable settings (e.g. screenshot interval, output format, max cache size)

- Support JSON Lines for metadata which is suatable for ML datasets

- Use `CsvHelper` library for safer and more robust CSV generation

- Leverage `AutoFixture` and `AutoNSubstitute` for cleaner setup in unit tests
