using Silk.NET.Input;
using System.Numerics;

namespace ECSEngineTest.Input;

internal class InputManager
{
    private readonly IInputContext _inputContext;
    private readonly Dictionary<IMouse, List<MouseButton>> _pressedMouseButtons = [];
    private readonly Dictionary<IKeyboard, List<Key>> _pressedKeys = [];

    public InputManager(IInputContext inputContext)
    {
        _inputContext = inputContext;

        SetupMouseEvents();
        SetupKeyboardEvents();
    }

    private void SetupMouseEvents()
    {
        foreach (var mouse in _inputContext.Mice)
        {
            mouse.DoubleClickTime = 200;

            mouse.MouseDown   += OnMouseDown;
            mouse.MouseUp     += OnMouseUp;
            mouse.Click       += OnClick;
            mouse.DoubleClick += OnDoubleClick;
            mouse.Scroll      += OnMouseScroll;
            mouse.MouseMove   += OnMouseMove;
        }
    }

    private void SetupKeyboardEvents()
    {
        foreach (var keyboard in _inputContext.Keyboards)
        {
            keyboard.KeyDown += OnKeyDown;
            keyboard.KeyUp   += OnKeyUp;
        }
    }

    private void OnMouseDown(IMouse mouse, Silk.NET.Input.MouseButton button)
    {
        var val = (MouseButton)(int)button;
        if (_pressedMouseButtons.TryGetValue(mouse, out var buttons))
        {
            buttons.Add(val);
        }
        else
        {
            buttons = [ val ];
            _pressedMouseButtons[mouse] = buttons;
        }

        EventManager.RaiseEvent(EventTypeFlags.MouseDown, new EventRaiseDto { Sender = this, Data = [ buttons.ToArray() ] });
    }

    private void OnMouseUp(IMouse mouse, Silk.NET.Input.MouseButton button)
    {
        var val = (MouseButton)(int)button;
        if (_pressedMouseButtons.TryGetValue(mouse, out var buttons))
        {
            buttons.Remove(val);
        }
        else
        {
            buttons = [];
            _pressedMouseButtons[mouse] = buttons;
        }

        EventManager.RaiseEvent(EventTypeFlags.MouseUp, new EventRaiseDto { Sender = this, Data = [ val ] });
    }

    private void OnClick(IMouse mouse, Silk.NET.Input.MouseButton button, Vector2 position)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseClick, new EventRaiseDto { Sender = this, Data = [ position, (MouseButton)(int)button] });
    }

    private void OnDoubleClick(IMouse mouse, Silk.NET.Input.MouseButton button, Vector2 vector)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseDoubleClick, new EventRaiseDto { Sender = this, Data = [vector, (MouseButton)(int)button] });
    }

    private void OnMouseScroll(IMouse mouse, ScrollWheel wheel)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseScroll, new EventRaiseDto { Sender = this, Data = [(int)wheel.X, (int)wheel.Y] });
    }

    private void OnMouseMove(IMouse mouse, Vector2 newPos)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseMove, new EventRaiseDto { Sender = this, Data = [ newPos ] });
    }

    private void OnKeyDown(IKeyboard keyboard, Silk.NET.Input.Key key, int code)
    {
        var val = (Key)(int)key;
        if (_pressedKeys.TryGetValue(keyboard, out var keys))
        {
            keys.Add(val);
        }
        else
        {
            keys = [ val ];
            _pressedKeys[keyboard] = keys;
        }

        EventManager.RaiseEvent(EventTypeFlags.KeyDown, new EventRaiseDto { Sender = this, Data = [ keys.ToArray() ] });
    }

    private void OnKeyUp(IKeyboard keyboard, Silk.NET.Input.Key key, int arg3)
    {
        var val = (Key)(int)key;
        if (_pressedKeys.TryGetValue(keyboard, out var keys))
        {
            keys.Remove(val);
        }
        else
        {
            keys = [];
            _pressedKeys[keyboard] = keys;
        }

        EventManager.RaiseEvent(EventTypeFlags.KeyUp, new EventRaiseDto { Sender = this, Data = [ val ] });
    }
}
