using System.Diagnostics.CodeAnalysis;

namespace KameRecorder.Models;

[ExcludeFromCodeCoverage]
public class KeyPressedEventArgs : EventArgs
{
	public string Key { get; }

	public KeyPressedEventArgs(string key)
	{
		Key = key;
	}
}