using KameRecorder.Abstractions;

namespace KameRecorder.Services;

public class ScreenshotCapturer : IScreenshotCapturer
{
	public Bitmap CaptureScreenshot()
	{
		var screenBounds = Screen.PrimaryScreen!.Bounds;
		var screenshot = new Bitmap(screenBounds.Width, screenBounds.Height);

		using var graphics = Graphics.FromImage(screenshot);
		graphics.CopyFromScreen(sourceX: 0, 
								sourceY: 0, 
								destinationX: 0,
								destinationY: 0, 
								blockRegionSize: screenshot.Size);

		return screenshot;
	}
}