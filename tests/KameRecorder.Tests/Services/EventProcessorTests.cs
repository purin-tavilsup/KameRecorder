using System.Drawing;
using KameRecorder.Abstractions;
using KameRecorder.Models;
using KameRecorder.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KameRecorder.Tests.Services;

public class EventProcessorTests
{
	[Fact]
    public async Task EnqueueEvent_MouseClickEvent_ShouldCallExpectedServices()
    {
        // Arrange
        var screenshot = new Bitmap(10, 10);
        var screenshotService = Substitute.For<IScreenshotService>();
        var evenLogger = Substitute.For<IEventLogger>();
		var logger = Substitute.For<ILogger<EventProcessor>>();
		var timestamp = new DateTime(2025, 7, 11, 15, 42, 37, 123);
        
        screenshotService.GetBefore(Arg.Any<DateTime>())
                         .Returns(screenshot);
        
        var sut = new EventProcessor(screenshotService, evenLogger, logger);
		
        // Act
		sut.Start();
        sut.EnqueueEvent(EventType.MouseClick, "Left", timestamp, 100, 200);
		
        await Task.Delay(300); // Wait for processing

        sut.Stop();
        
        await Task.Delay(100); // Let the event channel close gracefully

        // Assert
        await evenLogger.Received(1).SaveMetadataAsync(Arg.Any<KameEvent>(), Arg.Any<string>());
		screenshotService.Received(1).GetBefore(timestamp);
        evenLogger.Received(1).SaveScreenshot(Arg.Any<Bitmap>(), timestamp);
    }

    [Fact]
    public async Task EnqueueEvent_KeyPressEvent_ShouldCallExpectedServices()
    {
        // Arrange
        var screenshot = new Bitmap(10, 10);
        var screenshotService = Substitute.For<IScreenshotService>();
		var eventLogger = Substitute.For<IEventLogger>();
		var logger = Substitute.For<ILogger<EventProcessor>>();
		var timestamp = DateTime.UtcNow;
		
        screenshotService.GetLatest()
                         .Returns(screenshot);
		
        var sut = new EventProcessor(screenshotService, eventLogger, logger);
		
        // Act
		sut.Start();
        sut.EnqueueEvent(EventType.KeyPress, "A", timestamp);
		
        await Task.Delay(300); // Wait for processing

        sut.Stop();
		
        await Task.Delay(100); // Let the event channel close gracefully

        // Assert
		await eventLogger.Received(1).SaveMetadataAsync(Arg.Any<KameEvent>(), Arg.Any<string>());
        screenshotService.Received(1).GetLatest();
        eventLogger.Received(1).SaveScreenshot(Arg.Any<Bitmap>(), timestamp);
    }
}