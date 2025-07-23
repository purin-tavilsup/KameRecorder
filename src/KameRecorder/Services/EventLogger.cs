using System.Drawing.Imaging;
using System.IO.Abstractions;
using System.Text;
using KameRecorder.Abstractions;
using KameRecorder.Extensions;
using KameRecorder.Models;

namespace KameRecorder.Services;

public class EventLogger : IEventLogger
{
	private readonly IFileSystem _fileSystem;
	private readonly string _screenshotDirectory;
	private readonly string _metadataFile;
	private const string OutputDirectoryName = "output";
	private const string ScreenshotDirectoryName = "screenshots";
	private const string MetadataFileName = "metadata.csv";
	private const string MetadataCsvColumns =
		"EventType,Timestamp,Key,MouseButton,MouseLocationX,MouseLocationY,ScreenshotFileName";

	public EventLogger(IFileSystem fileSystem, string? outputDirectory = null)
	{
		_fileSystem = fileSystem;
		var outputAbsolutePath = outputDirectory ?? _fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OutputDirectoryName);
		_screenshotDirectory = _fileSystem.Path.Combine(outputAbsolutePath, ScreenshotDirectoryName);
		_metadataFile = _fileSystem.Path.Combine(outputAbsolutePath, MetadataFileName);

		_fileSystem.Directory.CreateDirectory(outputAbsolutePath);
		_fileSystem.Directory.CreateDirectory(_screenshotDirectory);

		EnsureMetadataFileExists();
	}
	
	public async Task SaveMetadataAsync(KameEvent kameEvent, string screenshotFilename)
	{
		EnsureMetadataFileExists();
		
		var metadata = kameEvent.ToMetadata(screenshotFilename);
		var stringBuilder = new StringBuilder();
		stringBuilder.Append($"{metadata.Type},");
		stringBuilder.Append($"{metadata.Timestamp},");
		stringBuilder.Append($"{metadata.Key},");
		stringBuilder.Append($"{metadata.MouseButton},");
		stringBuilder.Append($"{metadata.MouseLocationX},");
		stringBuilder.Append($"{metadata.MouseLocationY},");
		stringBuilder.Append($"{metadata.ScreenshotFilename}");
		
		var newEntry = stringBuilder.ToString();
		
		await _fileSystem.File.AppendAllTextAsync(_metadataFile, newEntry + Environment.NewLine);
	}

	private void EnsureMetadataFileExists()
	{
		if (!_fileSystem.File.Exists(_metadataFile))
		{
			_fileSystem.File.WriteAllText(_metadataFile, MetadataCsvColumns + Environment.NewLine);
		}
	}
	
	public string SaveScreenshot(Bitmap screenshot, DateTime timestamp)
	{
		var filename = $"screenshot_{timestamp:yyyyMMdd_HHmmssfff}.png";
		var fullPath = _fileSystem.Path.Combine(_screenshotDirectory, filename);

		using var stream = _fileSystem.FileStream.New(fullPath, FileMode.Create, FileAccess.Write);
		screenshot.Save(stream, ImageFormat.Png);

		return filename;
	}
}