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
        if (data == null || data.Data is null && !(eventType == EventTypeFlags.WindowLoaded || eventType == EventTypeFlags.WindowClosing))
            return;

        switch (eventType)
        {
            case EventTypeFlags.MouseMove:
                InvokeEvent(MouseMove, data.Sender, new MouseMoveEventArgs((Vector2)data.Data![0]));
                break;
            case EventTypeFlags.MouseScroll:
                InvokeEvent(MouseScroll, data.Sender, new MouseScrollEventArgs((int)data.Data![0], (int)data.Data[1]));
                break;
            case EventTypeFlags.MouseUp:
                InvokeEvent(MouseUp, data.Sender, new MouseUpEventArgs((MouseButton)data.Data![0]));
                break;
            case EventTypeFlags.MouseDown:
                InvokeEvent(MouseDown, data.Sender, new MouseDownEventArgs((MouseButton[])data.Data![0]));
                break;
            case EventTypeFlags.MouseClick:
                InvokeEvent(MouseClick, data.Sender, new MouseClickEventArgs((Vector2)data.Data![0], (MouseButton)data.Data[1]));
                break;
            case EventTypeFlags.MouseDoubleClick:
                InvokeEvent(MouseDoubleClick, data.Sender, new MouseClickEventArgs((Vector2)data.Data![0], (MouseButton)data.Data[1]));
                break;
            case EventTypeFlags.KeyUp:
                InvokeEvent(KeyUp, data.Sender, new KeyUpEventArgs((Key)data.Data![0]));
                break;
            case EventTypeFlags.KeyDown:
                InvokeEvent(KeyDown, data.Sender, new KeyDownEventArgs((Key[])data.Data![0]));
                break;
            //case EventTypeFlags.InputConnectionChanged:
            //    InputConnectionChanged?.Invoke(data.Sender, new InputConnectionChangedEventArgs((InputConnection)data.Data));
            //    break;
            //case EventTypeFlags.InputEvent:
            //    InputEvent?.Invoke(data.Sender, new InputEventArgs(eventType, (IInput)data.Data));
            //    break;
            case EventTypeFlags.WindowLoaded:
                InvokeEvent(WindowLoaded, data.Sender, new WindowLoadedEventArgs());
                break;
            case EventTypeFlags.WindowResized:
                InvokeEvent(WindowResized, data.Sender, new WindowResizedEventArgs((Vector2)data.Data![0]));
                break;
            case EventTypeFlags.WindowFileDrop:
                InvokeEvent(WindowFileDrop, data.Sender, new WindowFileDropEventArgs((string[])data.Data![0]));
                break;
            case EventTypeFlags.WindowClosing:
                InvokeEvent(WindowClosing, data.Sender, new WindowClosingEventArgs());
                break;
        }
    }

    private static void InvokeEvent<T>(EventHandler<T> handler, object? sender, T args) where T : EventArgs
    {
        if (handler is null) return;

        var delegates = handler.GetInvocationList();
        var layers = delegates.Where(x => x.Target is Layer)
                              .OrderByDescending(x => ((Layer)x.Target!).Order)
                              .ToArray();

        var others = delegates.Where(x => !layers.Contains(x)).ToArray();

        foreach (var @event in layers)
        {
            if (!((Layer)@event.Target!).Enabled)
                continue;

            @event.DynamicInvoke(sender, args);

            if (args is CancelEventArgs c && c.Cancel)
                break;
        }

        {
            if (args is CancelEventArgs c)
                c.Cancel = false;
        }

        foreach (var @event in others)
        {
            @event.DynamicInvoke(sender, args);

            if (args is CancelEventArgs c && c.Cancel)
                break;
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

public class MouseDownEventArgs(params MouseButton[] buttons) : CancelEventArgs
{
    public MouseButton[] Buttons { get; } = buttons;
}

public class MouseClickEventArgs(Vector2 position, MouseButton button) : CancelEventArgs
{
    public MouseButton Button { get; } = button;
    public Vector2 Position { get; } = position;
}

public class MouseEventArgs(EventTypeFlags eventType, Vector2 position, int x, int y, params MouseButton[] buttons) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public MouseButton[] Buttons { get; } = buttons;
    public Vector2 Position { get; } = position;
    public int ScrollX { get; } = x;
    public int ScrollY { get; } = y;
}

public class KeyUpEventArgs(Key key) : CancelEventArgs
{
    public Key Key { get; } = key;
}

public class KeyDownEventArgs(params Key[] keys) : CancelEventArgs
{
    public Key[] Keys { get; } = keys;
}

public class KeyboardEventArgs(EventTypeFlags eventType, Key keyUp, params Key[] keysPressed) : CancelEventArgs
{
    public EventTypeFlags EventType { get; } = eventType;
    public Key KeyUp { get; } = keyUp;
    public Key[] KeysPressed { get; } = keysPressed;
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
