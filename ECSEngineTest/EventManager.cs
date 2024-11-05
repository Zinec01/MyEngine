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
    public static event EventHandler<KeyboardEventArgs> KeyboardEvent;

    //public static event EventHandler<InputConnectionChangedEventArgs> InputConnectionChanged;
    //public static event EventHandler<InputEventArgs> InputEvent;

    public static event EventHandler<WindowLoadedEventArgs> WindowLoaded;
    public static event EventHandler<WindowResizedEventArgs> WindowResized;
    public static event EventHandler<WindowFileDropEventArgs> WindowFileDrop;
    public static event EventHandler<WindowClosingEventArgs> WindowClosing;
    public static event EventHandler<WindowEventArgs> WindowEvent;

    internal static void RaiseEvent(EventTypeFlags eventType, EventRaiseDto data)
    {
        if (data.Data == null) return;

        switch (eventType)
        {
            case EventTypeFlags.MouseMove:
                InvokeEvent(MouseMove, data.Sender, new MouseMoveEventArgs((Vector2)data.Data[0]));
                break;
            case EventTypeFlags.MouseScroll:
                InvokeEvent(MouseScroll, data.Sender, new MouseScrollEventArgs((int)data.Data[0], (int)data.Data[1]));
                break;
            case EventTypeFlags.MouseUp:
                InvokeEvent(MouseUp, data.Sender, new MouseUpEventArgs((MouseButton)data.Data[0]));
                break;
            case EventTypeFlags.MouseDown:
                InvokeEvent(MouseDown, data.Sender, new MouseDownEventArgs((MouseButton)data.Data[0]));
                break;
            case EventTypeFlags.MouseClick:
                InvokeEvent(MouseClick, data.Sender, new MouseClickEventArgs((MouseButton)data.Data[0], (Vector2)data.Data[1]));
                break;
            case EventTypeFlags.MouseDoubleClick:
                InvokeEvent(MouseDoubleClick, data.Sender, new MouseClickEventArgs((MouseButton)data.Data[0], (Vector2)data.Data[1]));
                break;
            case EventTypeFlags.MouseEvent:
                InvokeEvent(MouseEvent, data.Sender, new MouseEventArgs(eventType, (MouseButton)data.Data[0], (Vector2)data.Data[1], (int)data.Data[2], (int)data.Data[3]));
                break;
            case EventTypeFlags.KeyboardUp:
                InvokeEvent(KeyUp, data.Sender, new KeyUpEventArgs((Key)data.Data[0], (int)data.Data[1]));
                break;
            case EventTypeFlags.KeyboardDown:
                InvokeEvent(KeyDown, data.Sender, new KeyDownEventArgs((Key)data.Data[0], (int)data.Data[1]));
                break;
            case EventTypeFlags.KeyboardEvent:
                InvokeEvent(KeyboardEvent, data.Sender, new KeyboardEventArgs(eventType, (Key)data.Data[0], (int)data.Data[1]));
                break;
            //case EventTypeFlags.InputConnectionChanged:
            //    InputConnectionChanged?.Invoke(data.Sender, new InputConnectionChangedEventArgs((InputConnection)data.Data));
            //    break;
            //case EventTypeFlags.InputEvent:
            //    InputEvent?.Invoke(data.Sender, new InputEventArgs(eventType, (IInput)data.Data));
            //    break;
            //case EventTypeFlags.WindowLoaded:
            //    WindowLoaded?.Invoke(data.Sender, new WindowLoadedEventArgs());
            //    break;
            case EventTypeFlags.WindowResized:
                WindowResized?.Invoke(data.Sender, new WindowResizedEventArgs((Vector2)data.Data[0]));
                break;
        }
    }

    private static void InvokeEvent<T>(EventHandler<T> handler, object? sender, T args) where T : CancelEventArgs
    {
        if (handler is null) return;

        foreach (var @event in handler.GetInvocationList())
        {
            if (args.Cancel) break;
            @event.DynamicInvoke(sender, args);
        }
    }
}

public class MouseMoveEventArgs(Vector2 position) : CancelEventArgs
{
    public Vector2 Position { get; } = position;
}

public class MouseScrollEventArgs(int x, int y) : CancelEventArgs
{
    public int X { get; } = x;
    public int Y { get; } = y;
}

public class MouseUpEventArgs(MouseButton button) : CancelEventArgs
{
    public MouseButton Button { get; } = button;
}

public class MouseDownEventArgs(MouseButton button) : CancelEventArgs
{
    public MouseButton Button { get; } = button;
}

public class MouseClickEventArgs(MouseButton button, Vector2 position) : CancelEventArgs
{
    public MouseButton Button { get; } = button;
    public Vector2 Position { get; } = position;
}

public class MouseEventArgs(EventTypeFlags eventType, MouseButton button, Vector2 position, int x, int y) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public MouseButton Button { get; } = button;
    public Vector2 Position { get; } = position;
    public int ScrollX { get; } = x;
    public int ScrollY { get; } = y;
}

public class KeyUpEventArgs(Key key, int code) : CancelEventArgs
{
    public Key Key { get; } = key;
    public int Code { get; } = code;
}

public class KeyDownEventArgs(Key key, int code) : CancelEventArgs
{
    public Key Key { get; } = key;
    public int Code { get; } = code;
}

public class KeyboardEventArgs(EventTypeFlags eventType, Key key, int code) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Key Key { get; } = key;
    public int Code { get; } = code;
}

public class WindowLoadedEventArgs() : EventArgs { }

public class WindowResizedEventArgs(Vector2 newSize) : CancelEventArgs
{
    public Vector2 NewSize { get; } = newSize;
}

public class WindowFileDropEventArgs(string[] filePaths) : CancelEventArgs
{
    public string[] FilePaths { get; } = filePaths;
}

public class WindowClosingEventArgs() : EventArgs { }

public class WindowEventArgs(EventTypeFlags eventType, Vector2 newSize, string[] filePaths) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Vector2 NewSize { get; } = newSize;
    public string[] FilePaths { get; } = filePaths;
}
