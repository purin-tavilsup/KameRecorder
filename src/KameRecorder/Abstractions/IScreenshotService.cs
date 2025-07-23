namespace KameRecorder.Abstractions;

public interface IScreenshotService
{
	void Capture();
	
	Bitmap? GetLatest();
	
	Bitmap? GetBefore(DateTime timestamp);
	
	void ClearCache();
}