using KameRecorder.Abstractions;
using KameRecorder.Models;
using Microsoft.Extensions.Logging;

namespace KameRecorder.Services;

public class RecorderService : IRecorderService
{
	private const int MillisecondDelay = 200;
	private readonly IScreenshotService _screenshotService;
	private readonly IHookService _hookService;
	private readonly IEventProcessor _eventProcessor;
	private readonly ILogger<RecorderService> _logger;
	
	private Thread? _screenshotThread;
	private CancellationTokenSource? _cancellationTokenSource;
	
	private EventHandler<KeyPressedEventArgs>? _keyPressedHandler;
	private EventHandler<MouseClickEventArgs>? _mouseClickedHandler;
	
	public RecorderService(IEventProcessor eventProcessor, 
						   IHookService hookService, 
						   IScreenshotService screenshotService, 
						   ILogger<RecorderService> logger)
	{
		_eventProcessor = eventProcessor;
		_hookService = hookService;
		_screenshotService = screenshotService;
		_logger = logger;
	}
	
	public void Start()
	{
		_logger.LogInformation("RecorderService started.");

		StartScreenshotThread();
		_eventProcessor.Start();
		SubscribeToKeyPressedEvent();
		SubscribeToMouseClickedEvent();
		_hookService.Start();
	}

	private void SubscribeToKeyPressedEvent()
	{
		_keyPressedHandler = (_, e) =>
		{
			_eventProcessor.EnqueueEvent(EventType.KeyPress, e.Key, DateTime.UtcNow);
		};

		_hookService.KeyPressed += _keyPressedHandler;
	}

	private void SubscribeToMouseClickedEvent()
	{
		_mouseClickedHandler = (_, e) =>
		{
			_eventProcessor.EnqueueEvent(EventType.MouseClick, e.Button, DateTime.UtcNow, e.X, e.Y);
		};

		_hookService.MouseClicked += _mouseClickedHandler;
	}

	private void UnsubscribeToKeyPressedEvent()
	{
		if (_keyPressedHandler is null)
		{
			return;
		}
			
		_hookService.KeyPressed -= _keyPressedHandler;
	}

	private void UnsubscribeToMouseClickedEvent()
	{
		if (_mouseClickedHandler is null)
		{
			return;
		}
		
		_hookService.MouseClicked -= _mouseClickedHandler;
	}

	private void StartScreenshotThread()
	{
		_cancellationTokenSource = new CancellationTokenSource();
		_screenshotThread = new Thread(() => CaptureScreenshotsLoop(_cancellationTokenSource.Token))
		{
			IsBackground = true
		};
		_screenshotThread.Start();
	}
	
	public void Stop()
	{
		_logger.LogInformation("RecorderService stopping...");

		_cancellationTokenSource?.Cancel();

		UnsubscribeToKeyPressedEvent();
		UnsubscribeToMouseClickedEvent();
		
		_hookService.Stop();
		_eventProcessor.Stop();
		_screenshotThread?.Join();

		_logger.LogInformation("RecorderService stopped.");
	}

	private static void Delay(int milliseconds)
	{
		Thread.Sleep(milliseconds);
	}
	
	private void CaptureScreenshotsLoop(CancellationToken token)
	{
		_logger.LogInformation("Screenshot thread running...");

		while (!token.IsCancellationRequested)
		{
			try
			{
				_screenshotService.Capture();
				
				Delay(MillisecondDelay);
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "An error occured while capturing screenshot. {Message}", ex.Message);
			}
		}
		
		_logger.LogInformation("Screenshot thread stopped.");
	}
}