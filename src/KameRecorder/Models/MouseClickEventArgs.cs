using System.Diagnostics.CodeAnalysis;

namespace KameRecorder.Models;

[ExcludeFromCodeCoverage]
public class MouseClickEventArgs: EventArgs
{
	public string Button { get; }
	
	public int X { get; }
	
	public int Y { get; }

	public MouseClickEventArgs(string button, int x, int y)
	{
		Button = button;
		X = x;
		Y = y;
	}
}