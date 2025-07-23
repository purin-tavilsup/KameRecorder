using System.Threading.Channels;
using KameRecorder.Abstractions;
using KameRecorder.Extensions;
using KameRecorder.Models;
using Microsoft.Extensions.Logging;

namespace KameRecorder.Services;

public class EventProcessor : IEventProcessor
{
	private const int MillisecondDelay = 200;
	private readonly IScreenshotService _screenshotService;
	private readonly IEventLogger _eventLogger;
	private readonly ILogger<EventProcessor> _logger;
	private readonly Channel<KameEvent> _eventChannel = Channel.CreateUnbounded<KameEvent>();

	public EventProcessor(IScreenshotService screenshotService, 
						  IEventLogger eventLogger, 
						  ILogger<EventProcessor> logger)
	{
		_screenshotService = screenshotService;
		_eventLogger = eventLogger;
		_logger = logger;
	}
	
	public void Start()
	{
		Task.Run(ProcessEventsAsync);
	}

	public void Stop()
	{
		_eventChannel.Writer.Complete();
		_screenshotService.ClearCache();
	}
	
	public void EnqueueEvent(EventType type, string inputDetail, DateTime timestamp, int? x = null, int? y = null)
	{
		var kameEvent = new KameEvent
		{
			Timestamp = timestamp,
			Type = type,
			InputDetail = inputDetail,
			X = x,
			Y = y
		};

		_eventChannel.Writer.TryWrite(kameEvent);
	}
	
	private async Task ProcessEventsAsync()
	{
		_logger.LogInformation("Event processing thread started.");

		await foreach (var kameEvent in _eventChannel.Reader.ReadAllAsync())
		{
			try
			{
				var screenshot = await GetScreenshotAsync(kameEvent);
				var screenshotFilename = SaveScreenshot(screenshot, kameEvent);
				await SaveMetadataAsync(kameEvent, screenshotFilename);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "An error occured while processing event. {Message}", ex.Message);
			}
		}
		
		_logger.LogInformation("Event processing thread stopped.");
	}

	private async Task<Bitmap?> GetScreenshotAsync(KameEvent kameEvent)
	{
		return kameEvent.Type switch
		{
			EventType.MouseClick => GetLastScreenshotBeforeEvent(kameEvent),
			EventType.KeyPress   => await GetNextScreenshotAsync(),
			_                    => null
		};
	}

	private string SaveScreenshot(Bitmap? screenshot, KameEvent kameEvent)
	{
		if (screenshot is null)
		{
			return string.Empty;
		}

		if (kameEvent is {Type: EventType.MouseClick, X: not null, Y: not null})
		{
			screenshot = screenshot.DrawCursor(kameEvent.X.Value, kameEvent.Y.Value);
		}
		
		var fileName = _eventLogger.SaveScreenshot(screenshot, kameEvent.Timestamp);
		
		return fileName;
	}

	private async Task SaveMetadataAsync(KameEvent kameEvent, string screenshotFilename)
	{
		await _eventLogger.SaveMetadataAsync(kameEvent, screenshotFilename);
	}

	private Bitmap? GetLastScreenshotBeforeEvent(KameEvent kameEvent)
	{
		return _screenshotService.GetBefore(kameEvent.Timestamp);
	}
	
	private async Task<Bitmap?> GetNextScreenshotAsync()
	{
		// Delay to ensure the next screenshot is taken
		await Task.Delay(MillisecondDelay);
		
		return _screenshotService.GetLatest();
	}
}