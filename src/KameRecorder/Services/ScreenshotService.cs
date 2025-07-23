using KameRecorder.Abstractions;
using KameRecorder.Utils;

namespace KameRecorder.Services;

public class ScreenshotService : IScreenshotService
{
	private readonly IScreenshotCapturer _screenshotCapturer;
	private readonly ScreenshotCache _cache = new();
	
	public ScreenshotService(IScreenshotCapturer screenshotCapturer)
	{
		_screenshotCapturer = screenshotCapturer;
	}

	public void Capture()
	{
		var screenshot = _screenshotCapturer.CaptureScreenshot();
		
		_cache.Add(screenshot);
		
		screenshot.Dispose();
	}

	public Bitmap? GetLatest()
	{
		return _cache.GetLatest()?.screenshot;
	}

	public Bitmap? GetBefore(DateTime timestamp)
	{
		return _cache.GetBefore(timestamp)?.screenshot;
	}

	public void ClearCache()
	{
		_cache.Clear();
	}
}