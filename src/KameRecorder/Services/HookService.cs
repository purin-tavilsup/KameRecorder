using Gma.System.MouseKeyHook;
using KameRecorder.Abstractions;
using KameRecorder.Models;

namespace KameRecorder.Services;

public class HookService : IHookService
{
	private IKeyboardMouseEvents? _globalHook;
	
	public event EventHandler<KeyPressedEventArgs>? KeyPressed;
	public event EventHandler<MouseClickEventArgs>? MouseClicked;
	
	private KeyPressEventHandler? _keyPressHandler;
	private MouseEventHandler? _mouseDownHandler;
	
	public void Start()
	{
		if (_globalHook is not null)
		{
			return;
		}
		
		_globalHook = Hook.GlobalEvents();
		SubscribeToKeyPressedEvent(_globalHook);
		SubscribeToMouseDownEvent(_globalHook);
	}

	private void SubscribeToKeyPressedEvent(IKeyboardMouseEvents globalHook)
	{
		_keyPressHandler = (_, e) =>
		{
			var eventArgs = new KeyPressedEventArgs(e.KeyChar.ToString());
			KeyPressed?.Invoke(this, eventArgs);
		};
		
		globalHook.KeyPress += _keyPressHandler;
	}

	private void SubscribeToMouseDownEvent(IKeyboardMouseEvents globalHook)
	{
		_mouseDownHandler = (_, e) =>
		{
			var eventArgs = new MouseClickEventArgs(e.Button.ToString(), e.X, e.Y);
			MouseClicked?.Invoke(this, eventArgs);
		};
		
		globalHook.MouseDown += _mouseDownHandler;
	}

	private void UnsubscribeToKeyPressedEvent(IKeyboardMouseEvents globalHook)
	{
		if (_keyPressHandler is null)
		{
			return;
		}
		
		globalHook.KeyPress -= _keyPressHandler;
		_keyPressHandler = null;
	}

	private void UnsubscribeToMouseDownEvent(IKeyboardMouseEvents globalHook)
	{
		if (_mouseDownHandler is null)
		{
			return;
		}
		
		globalHook.MouseDown -= _mouseDownHandler;
		_mouseDownHandler = null;
	}
	
	public void Stop()
	{
		if (_globalHook is null)
		{
			return;
		}
		
		UnsubscribeToKeyPressedEvent(_globalHook);
		UnsubscribeToMouseDownEvent(_globalHook);
			
		_globalHook.Dispose();
		_globalHook = null;
	}
}