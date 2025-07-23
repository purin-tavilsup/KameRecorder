using System.Diagnostics.CodeAnalysis;

namespace KameRecorder.Models;

[ExcludeFromCodeCoverage]
public class KameEvent
{
	public DateTime Timestamp { get; init; }
	
	public EventType Type { get; init; }
	
	public required string InputDetail { get; init; }
	
	public int? X { get; init; }
	
	public int? Y { get; init; }
}