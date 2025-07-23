namespace KameRecorder.Utils;

public class ScreenshotCache
{
	private readonly int _maxSize;
	private readonly LinkedList<(DateTime timestamp, Bitmap screenshot)> _buffer = [];
	private object _lock = new();

	public ScreenshotCache(int maxSize = 50)
	{
		_maxSize = maxSize;
	}

	public void Add(Bitmap screenshot)
	{
		var copy = (Bitmap)screenshot.Clone();

		lock (_lock)
		{
			_buffer.AddLast((DateTime.UtcNow, copy));

			if (_buffer.Count <= _maxSize)
			{
				return;
			}
		
			var oldest = _buffer.First;
			oldest?.Value.screenshot.Dispose();
			_buffer.RemoveFirst();
		}
	}

	public (DateTime timestamp, Bitmap screenshot)? GetLatest()
	{
		lock (_lock)
		{
			return _buffer.Last?.Value;
		}
	}

	public (DateTime timestamp, Bitmap screenshot)? GetBefore(DateTime target)
	{
		lock (_lock)
		{
			for (var node = _buffer.Last; node is not null; node = node.Previous)
			{
				if (node.Value.timestamp <= target)
				{
					return node.Value;
				}
			}
		
			return null;
		}
	}

	public void Clear()
	{
		lock (_lock)
		{
			foreach (var (_, screenshot) in _buffer)
			{
				screenshot.Dispose();
			}

			_buffer.Clear();
		}
	}
}