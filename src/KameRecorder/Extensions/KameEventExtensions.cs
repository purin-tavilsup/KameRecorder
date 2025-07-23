using System.Globalization;
using KameRecorder.Models;

namespace KameRecorder.Extensions;

public static class KameEventExtensions
{
	public static EventMetadata ToMetadata(this KameEvent kameEvent, string screenshotFilename)
	{
		return new EventMetadata
		{
			Type = kameEvent.Type.ToString(),
			Timestamp = kameEvent.GetTimestamp(),
			Key = kameEvent.GetKey(),
			MouseButton = kameEvent.GetMouseButton(),
			MouseLocationX = kameEvent.GetMouseLocationX(),
			MouseLocationY = kameEvent.GetMouseLocationY(),
			ScreenshotFilename = screenshotFilename
		};
	}

	private static string GetTimestamp(this KameEvent kameEvent)
	{
		// ISO 8601
		return kameEvent.Timestamp.ToString("O", CultureInfo.InvariantCulture);
	}
	
	private static string GetKey(this KameEvent kameEvent)
	{
		return kameEvent.Type == EventType.KeyPress ? kameEvent.InputDetail : string.Empty;
	}

	private static string GetMouseButton(this KameEvent kameEvent)
	{
		return kameEvent.Type == EventType.MouseClick ? kameEvent.InputDetail : string.Empty;
	}

	private static string GetMouseLocationX(this KameEvent kameEvent)
	{
		if (kameEvent.Type != EventType.MouseClick || kameEvent.X is null)
		{
			return string.Empty;
		}

		return kameEvent.X.Value.ToString();
	}
	
	private static string GetMouseLocationY(this KameEvent kameEvent)
	{
		if (kameEvent.Type != EventType.MouseClick || kameEvent.Y is null)
		{
			return string.Empty;
		}

		return kameEvent.Y.Value.ToString();
	}
}