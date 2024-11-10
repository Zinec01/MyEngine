using ECSEngineTest.Input;
using System.ComponentModel;
using System.Numerics;

namespace ECSEngineTest;

public static class EventManager
{
    public static event EventHandler<MouseMoveEventArgs> MouseMove;
    public static event EventHandler<MouseScrollEventArgs> MouseScroll;
    public static event EventHandler<MouseUpEventArgs> MouseUp;
    public static event EventHandler<MouseDownEventArgs> MouseDown;
    public static event EventHandler<MouseClickEventArgs> MouseClick;
    public static event EventHandler<MouseClickEventArgs> MouseDoubleClick;
    public static event EventHandler<MouseEventArgs> MouseEvent;

    public static event EventHandler<KeyUpEventArgs> KeyUp;
    public static event EventHandler<KeyDownEventArgs> KeyDown;
    public static event EventHandler<KeyCharEventArgs> KeyChar;
    public static event EventHandler<KeyboardEventArgs> KeyboardEvent;

    public static event EventHandler<InputDeviceConnectionChangedEventArgs> InputDeviceConnectionChanged;

    public static event EventHandler<WindowLoadedEventArgs> WindowLoaded;
    public static event EventHandler<WindowResizedEventArgs> WindowResized;
    public static event EventHandler<WindowFileDropEventArgs> WindowFileDrop;
    public static event EventHandler<WindowClosingEventArgs> WindowClosing;
    public static event EventHandler<WindowEventArgs> WindowEvent;

    internal static void RaiseEvent(EventTypeFlags eventType, EventRaiseDto data)
    {
        if (data == null) return;

        LayerManager.RaiseEvent(eventType, new EventEventArgs(data.Device, data.MouseButton, data.Key, data.Window, data.FilePaths));

        switch (eventType)
        {
            case EventTypeFlags.MouseMove:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseMove, data.Sender, new MouseMoveEventArgs(mouse, mouse.Position));
                break;
            }
            case EventTypeFlags.MouseScroll:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseScroll, data.Sender, new MouseScrollEventArgs(mouse, mouse.Scroll));
                break;
            }
            case EventTypeFlags.MouseUp:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseUp, data.Sender, new MouseUpEventArgs(mouse, data.MouseButton));
                break;
            }
            case EventTypeFlags.MouseDown:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseDown, data.Sender, new MouseDownEventArgs(mouse, data.MouseButton));
                break;
            }
            case EventTypeFlags.MouseClick:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseClick, data.Sender, new MouseClickEventArgs(mouse, mouse.Position, data.MouseButton));
                break;
            }
            case EventTypeFlags.MouseDoubleClick:
            {
                if (data.Device is not Mouse mouse) return;
                InvokeEvent(MouseDoubleClick, data.Sender, new MouseClickEventArgs(mouse, mouse.Position, data.MouseButton));
                break;
            }
            case EventTypeFlags.KeyUp:
            {
                if (data.Device is not Keyboard keyboard) return;
                InvokeEvent(KeyUp, data.Sender, new KeyUpEventArgs(keyboard, data.Key));
                break;
            }
            case EventTypeFlags.KeyDown:
            {
                if (data.Device is not Keyboard keyboard) return;
                InvokeEvent(KeyDown, data.Sender, new KeyDownEventArgs(keyboard, data.Key));
                break;
            }
            case EventTypeFlags.KeyChar:
            {
                if (data.Device is not Keyboard keyboard) return;
                InvokeEvent(KeyChar, data.Sender, new KeyCharEventArgs(keyboard, data.Char));
                break;
            }
            case EventTypeFlags.InputDeviceConnectionChanged:
            {
                InvokeEvent(InputDeviceConnectionChanged, data.Sender, new InputDeviceConnectionChangedEventArgs(data.Device, data.Device.IsConnected));
                break;
            }
            case EventTypeFlags.WindowLoaded:
            {
                InvokeEvent(WindowLoaded, data.Sender, new WindowLoadedEventArgs(data.Window));
                break;
            }
            case EventTypeFlags.WindowResized:
            {
                InvokeEvent(WindowResized, data.Sender, new WindowResizedEventArgs(data.Window, data.Window.Size));
                break;
            }
            case EventTypeFlags.WindowFileDrop:
            {
                InvokeEvent(WindowFileDrop, data.Sender, new WindowFileDropEventArgs(data.Window, data.FilePaths));
                break;
            }
            case EventTypeFlags.WindowClosing:
            {
                InvokeEvent(WindowClosing, data.Sender, new WindowClosingEventArgs(data.Window));
                break;
            }
        }
    }

    private static void InvokeEvent<T>(EventHandler<T> handler, object? sender, T args) where T : CancelEventArgs
    {
        if (handler is null) return;

        foreach (var @event in handler.GetInvocationList())
        {
            @event.DynamicInvoke(sender, args);

            if (args.Cancel) break;
        }
    }
}

public class MouseMoveEventArgs(Mouse mouse, Vector2 position) : CancelEventArgs
{
    public Mouse Mouse { get; } = mouse;
    public Vector2 Position { get; } = position;
}

public class MouseScrollEventArgs(Mouse mouse, Vector2 scroll) : CancelEventArgs
{
    public Mouse Mouse { get; } = mouse;
    public Vector2 Scroll { get; } = scroll;
}

public class MouseUpEventArgs(Mouse mouse, MouseButton button) : CancelEventArgs
{
    public Mouse Mouse { get; } = mouse;
    public MouseButton Button { get; } = button;
}

public class MouseDownEventArgs(Mouse mouse, MouseButton button) : CancelEventArgs
{
    public Mouse Mouse { get; } = mouse;
    public MouseButton Button { get; } = button;
}

public class MouseClickEventArgs(Mouse mouse, Vector2 position, MouseButton button) : CancelEventArgs
{
    public Mouse Mouse { get; } = mouse;
    public MouseButton Button { get; } = button;
    public Vector2 Position { get; } = position;
}

public class MouseEventArgs(EventTypeFlags eventType, Mouse mouse) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Mouse Mouse { get; } = mouse;
}

public class KeyUpEventArgs(Keyboard keyboard, Key key) : CancelEventArgs
{
    public Keyboard Keyboard { get; } = keyboard;
    public Key Key { get; } = key;
}

public class KeyDownEventArgs(Keyboard keyboard, Key key) : CancelEventArgs
{
    public Keyboard Keyboard { get; } = keyboard;
    public Key Key { get; } = key;
}

public class KeyCharEventArgs(Keyboard keyboard, char ch) : CancelEventArgs
{
    public Keyboard Keyboard { get; } = keyboard;
    public char Char { get; } = ch;
}

public class KeyboardEventArgs(EventTypeFlags eventType, Keyboard keyboard, Key key) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Keyboard Keyboard { get; } = keyboard;
    public Key Key { get; } = key;
}

public class InputDeviceConnectionChangedEventArgs(IInputDevice device, bool isConnected) : CancelEventArgs
{
    public IInputDevice Device { get; } = device;
    public bool IsConnected { get; } = isConnected;
}

public class WindowLoadedEventArgs(Window window) : CancelEventArgs
{
    public Window Window { get; } = window;
}

public class WindowResizedEventArgs(Window window, Vector2 size) : CancelEventArgs
{
    public Window Window { get; } = window;
    public Vector2 Size { get; } = size;
}

public class WindowFileDropEventArgs(Window window, string[] filePaths) : CancelEventArgs
{
    public Window Window { get; } = window;
    public string[] FilePaths { get; } = filePaths;
}

public class WindowClosingEventArgs(Window window) : CancelEventArgs
{
    public Window Window { get; } = window;
}

public class WindowEventArgs(Window window, EventTypeFlags eventType, string[] filePaths) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Window Window { get; } = window;
    public string[] FilePaths { get; } = filePaths;
}

public class EventEventArgs(IInputDevice device, MouseButton button, Key key, Window window, string[] filePaths) : CancelEventArgs
{
    public IInputDevice InputDevice { get; } = device;
    public MouseButton Button { get; } = button;
    public Key Key { get; } = key;
    public Window Window { get; } = window;
    public string[] FilePaths { get; } = filePaths;
}
