using System.Drawing;
using System.IO.Abstractions.TestingHelpers;
using KameRecorder.Models;
using KameRecorder.Services;
using Shouldly;

namespace KameRecorder.Tests.Services;

public class EventLoggerTests
{
	[Fact]
	public void SaveScreenshot_ShouldCreatePngFile_WithExpectedName()
	{
		// Arrange
		var mockFileSystem = new MockFileSystem();
		const string outputDirectory = @"C:\virtual\output";
		const string screenshotDirectory = @"C:\virtual\output\screenshots";
		var timestamp = new DateTime(2025, 7, 11, 14, 30, 15, DateTimeKind.Utc);
		var sut = new EventLogger(mockFileSystem, outputDirectory);
		
		using var screenshot = new Bitmap(10, 10);
		using var graphics = Graphics.FromImage(screenshot);
		graphics.Clear(Color.Green);

		// Act
		var filename = sut.SaveScreenshot(screenshot, timestamp);

		// Assert
		var expectedFileName = $"screenshot_{timestamp:yyyyMMdd_HHmmssfff}.png";
		filename.ShouldBe(expectedFileName);

		var expectedFilePath = $@"{screenshotDirectory}\{expectedFileName}";
		mockFileSystem.FileExists(expectedFilePath).ShouldBeTrue();

		var imageBytes = mockFileSystem.GetFile(expectedFilePath).Contents;
		imageBytes.Length.ShouldBeGreaterThan(0);
	}
	
	[Fact]
	public async Task SaveMetadataAsync_ShouldCreateCsvFile_WithCorrectContent()
	{
		// Arrange
		var mockFileSystem = new MockFileSystem();
		const string outputDirectory = @"C:\virtual\output";
		const string expectedMetadataFilePath = @"C:\virtual\output\metadata.csv";
		const string screenshotFilename = "screenshot_20250711_154237123.png";
		var timestamp = new DateTime(2025, 7, 11, 15, 42, 37, 123, DateTimeKind.Utc);
		var sut = new EventLogger(mockFileSystem, outputDirectory);
		
		var mockEvent = new KameEvent
		{
			Type = EventType.MouseClick,
			Timestamp = timestamp,
			InputDetail = "Left",
			X = 300,
			Y = 400
		};

		// Act
		await sut.SaveMetadataAsync(mockEvent, screenshotFilename);

		// Assert
		mockFileSystem.FileExists(expectedMetadataFilePath)
					  .ShouldBeTrue();

		var content = await mockFileSystem.File.ReadAllTextAsync(expectedMetadataFilePath);
		var lines = content.Split(Environment.NewLine);

		lines.Length.ShouldBe(3); // header, 1 record, and empty final line
		lines[0].ShouldBe("EventType,Timestamp,Key,MouseButton,MouseLocationX,MouseLocationY,ScreenshotFileName");
	}
}