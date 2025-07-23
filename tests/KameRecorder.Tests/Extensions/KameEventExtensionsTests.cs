using KameRecorder.Extensions;
using KameRecorder.Models;
using Shouldly;

namespace KameRecorder.Tests.Extensions;

public class KameEventExtensionsTests
{
	[Fact]
    public void ToMetadata_MouseClickEvent_ShouldMapAllFieldsCorrectly()
    {
        // Arrange
        const string mouseButton = "Right";
        const string eventType = nameof(EventType.MouseClick);
        const string screenshotFilename = "20250711_123456789.png";
        const string expectedTimestamp = "2025-07-11T12:34:56.0000000Z";
        var timestamp = new DateTime(2025, 7, 11, 12, 34, 56, DateTimeKind.Utc);
        var kameEvent = new KameEvent
        {
            Type = EventType.MouseClick,
            Timestamp = timestamp,
            InputDetail = mouseButton,
            X = 200,
            Y = 100
        };
        
        // Act
        var result = kameEvent.ToMetadata(screenshotFilename);

        // Assert
        result.Type.ShouldBe(eventType);
        result.Timestamp.ShouldBe(expectedTimestamp);
        result.Key.ShouldBeEmpty();
        result.MouseButton.ShouldBe(mouseButton);
        result.MouseLocationX.ShouldBe(kameEvent.X.ToString());
        result.MouseLocationY.ShouldBe(kameEvent.Y.ToString());
        result.ScreenshotFilename.ShouldBe(screenshotFilename);
    }

    [Fact]
    public void ToMetadata_Should_Handle_KeyboardEvent_Without_MouseCoordinates()
    {
        // Arrange
        const string keyPressed = "A";
        const string eventType = nameof(EventType.KeyPress);
        const string screenshotFilename = "20250711_090000000.png";
        const string expectedTimestamp = "2025-07-11T09:00:00.0000000Z";
        var timestamp = new DateTime(2025, 7, 11, 9, 0, 0, DateTimeKind.Utc);
        var kameEvent = new KameEvent
        {
            Type = EventType.KeyPress,
            Timestamp = timestamp,
            InputDetail = keyPressed
        };
        
        // Act
        var result = kameEvent.ToMetadata(screenshotFilename);

        // Assert
        result.Type.ShouldBe(eventType);
        result.Timestamp.ShouldBe(expectedTimestamp);
        result.Key.ShouldBe(keyPressed);
        result.MouseButton.ShouldBeEmpty();
        result.MouseLocationX.ShouldBeEmpty();
        result.MouseLocationY.ShouldBeEmpty();
        result.ScreenshotFilename.ShouldBe(screenshotFilename);
    }
}