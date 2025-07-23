using System.IO.Abstractions;
using KameRecorder.Abstractions;
using KameRecorder.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KameRecorder;

static class Program
{
	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();
		
		var host = Host.CreateDefaultBuilder()
					   .ConfigureLogging(logging =>
					   {
						   logging.ClearProviders();
						   logging.AddConsole();
					   })
					   .ConfigureServices((_, services) =>
					   {
						   services.AddSingleton<IRecorderService, RecorderService>();
						   services.AddSingleton<IScreenshotCapturer , ScreenshotCapturer>();
						   services.AddSingleton<IScreenshotService, ScreenshotService>();
						   services.AddSingleton<IHookService, HookService>();
						   services.AddSingleton<IFileSystem, FileSystem>();
						   services.AddSingleton<IEventProcessor, EventProcessor>();
						   services.AddSingleton<IEventLogger, EventLogger>();
						   services.AddSingleton<MainForm>();
					   })
					   .Build();

		using var serviceScope = host.Services.CreateScope();
		var services = serviceScope.ServiceProvider;
		var mainForm = services.GetRequiredService<MainForm>();
		
		Application.Run(mainForm);
	}
}