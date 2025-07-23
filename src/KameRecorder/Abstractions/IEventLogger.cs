using KameRecorder.Models;

namespace KameRecorder.Abstractions;

public interface IEventLogger
{
	Task SaveMetadataAsync(KameEvent kameEvent, string screenshotFilename);
		
	string SaveScreenshot(Bitmap bmp, DateTime timestamp);
}