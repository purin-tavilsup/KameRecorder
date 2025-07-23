using KameRecorder.Models;

namespace KameRecorder.Abstractions;

public interface IEventProcessor
{
	void EnqueueEvent(EventType type, string inputDetail, DateTime timestamp, int? x = null, int? y = null);
	
	void Start();
	
	void Stop();
}