using KameRecorder.Abstractions;
using KameRecorder.Models;
using KameRecorder.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace KameRecorder.Tests.Services;

public class RecorderServiceTests
{
	[Fact]
	public void Start_ShouldStartRequiredServices()
	{
		// Arrange
		var processor = Substitute.For<IEventProcessor>();
		var hookService = Substitute.For<IHookService>();
		var screenshotService = Substitute.For<IScreenshotService>();
		var logger = Substitute.For<ILogger<RecorderService>>();

		var sut = new RecorderService(processor, hookService, screenshotService, logger);

		// Act
		sut.Start();

		// Assert
		hookService.Received(1).Start();
		processor.Received(1).Start();

		// Wait for the screenshot thread to start capturing
		Thread.Sleep(250);
		screenshotService.Received().Capture();

		sut.Stop();
	}
	
	[Fact]
	public void Stop_ShouldCallDependenciesAndCancelThread()
	{
		// Arrange
		var processor = Substitute.For<IEventProcessor>();
		var hookService = Substitute.For<IHookService>();
		var screenshotService = Substitute.For<IScreenshotService>();
		var logger = Substitute.For<ILogger<RecorderService>>();

		var sut = new RecorderService(processor, hookService, screenshotService, logger);
		sut.Start();

		// Act
		sut.Stop();

		// Assert
		hookService.Received(1).Stop();
		processor.Received(1).Stop();
	}
	
	[Fact]
	public void MouseClicked_ShouldTriggerEnqueueEvent()
	{
		// Arrange
		var processor = Substitute.For<IEventProcessor>();
		var hookService = Substitute.For<IHookService>();
		var screenshotService = Substitute.For<IScreenshotService>();
		var logger = Substitute.For<ILogger<RecorderService>>();
		const int x = 123;
		const int y = 456;
		const string clickedButton = "Left";

		var sut = new RecorderService(processor, hookService, screenshotService, logger);
		sut.Start();

		var mouseArgs = new MouseClickEventArgs(clickedButton, x, y);

		// Act
		hookService.MouseClicked += Raise.EventWith(null, mouseArgs);
		Thread.Sleep(50); // Let the event be processed

		// Assert
		processor.Received(1).EnqueueEvent(EventType.MouseClick, clickedButton, Arg.Any<DateTime>(), x, y);

		sut.Stop();
	}

	[Fact]
	public void KeyPressed_ShouldTriggerEnqueueEvent()
	{
		// Arrange
		var processor = Substitute.For<IEventProcessor>();
		var hookService = Substitute.For<IHookService>();
		var screenshotService = Substitute.For<IScreenshotService>();
		var logger = Substitute.For<ILogger<RecorderService>>();
		const string keyPressed = "A";

		var sut = new RecorderService(processor, hookService, screenshotService, logger);
		sut.Start();

		var keyArgs = new KeyPressedEventArgs(keyPressed);

		// Act
		hookService.KeyPressed += Raise.EventWith(null, keyArgs);
		Thread.Sleep(50); // Let the event be processed

		// Assert
		processor.Received(1).EnqueueEvent(EventType.KeyPress, keyPressed, Arg.Any<DateTime>());

		sut.Stop();
	}
}