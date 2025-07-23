using KameRecorder.Models;

namespace KameRecorder.Abstractions;

public interface IHookService
{
	event EventHandler<KeyPressedEventArgs> KeyPressed;
	
	event EventHandler<MouseClickEventArgs> MouseClicked;
	
	void Start();
	
	void Stop();
}