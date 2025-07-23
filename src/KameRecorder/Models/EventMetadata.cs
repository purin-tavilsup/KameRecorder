using System.Diagnostics.CodeAnalysis;

namespace KameRecorder.Models;

[ExcludeFromCodeCoverage]
public record EventMetadata
{
	public required string Timestamp { get; init; }
	
	public required string Type { get; init; }
	
	public string Key { get; init; } = string.Empty;
	
	public string MouseButton { get; init; } = string.Empty;
	
	public string MouseLocationX { get; init; } = string.Empty;
	
	public string MouseLocationY { get; init; } = string.Empty;
	
	public required string ScreenshotFilename { get; init; }
	
	
}