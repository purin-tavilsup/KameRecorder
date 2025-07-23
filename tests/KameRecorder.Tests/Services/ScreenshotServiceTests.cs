using System.Drawing;
using KameRecorder.Abstractions;
using KameRecorder.Services;
using NSubstitute;
using Shouldly;

namespace KameRecorder.Tests.Services;

public class ScreenshotServiceTests
{
	[Fact]
	public void Capture_ShouldAddScreenshotToCache()
	{
		// Arrange
		const int width = 100;
		const int height = 100;
		var mockBitmap = new Bitmap(width, height);
		var capturer = Substitute.For<IScreenshotCapturer>();
		
		capturer.CaptureScreenshot()
				.Returns(mockBitmap);

		var sut = new ScreenshotService(capturer);

		// Act
		sut.Capture();
		var screenshot = sut.GetLatest();

		// Assert
		screenshot.ShouldNotBeNull();
		screenshot.Width.ShouldBe(width);
		screenshot.Height.ShouldBe(height);

		sut.ClearCache();
		mockBitmap.Dispose();
	}

	[Fact]
	public void Clear_ShouldRemoveScreenshotsFromCache()
	{
		// Arrange
		const int width = 100;
		const int height = 100;
		var mockBitmap = new Bitmap(width, height);
		var capturer = Substitute.For<IScreenshotCapturer>();
		
		capturer.CaptureScreenshot()
				.Returns(mockBitmap);

		var sut = new ScreenshotService(capturer);

		// Act
		sut.Capture();
		sut.ClearCache();
		var screenshot = sut.GetLatest();

		// Assert
		screenshot.ShouldBeNull();

		mockBitmap.Dispose();
	}
}