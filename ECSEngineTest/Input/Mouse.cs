using Silk.NET.Input;
using System.Numerics;

namespace ECSEngineTest.Input;

public class Mouse : IInputDevice
{
    private readonly IMouse _silkMouse;
    private readonly List<MouseButton> _pressedButtons = [];

    public IReadOnlyList<MouseButton> PressedButtons => _pressedButtons;
    public int Index => _silkMouse.Index;
    public string Name => _silkMouse.Name;
    public bool IsConnected => _silkMouse.IsConnected;
    public int DoubleClickTime { get => _silkMouse.DoubleClickTime; set => _silkMouse.DoubleClickTime = value; }

    public Vector2 PreviousPosition { get; private set; }
    public Vector2 Position { get; private set; }
    public Vector2 Scroll { get; private set; }

    //public event Action<Mouse, Vector2>     MouseMove;
    //public event Action<Mouse, MouseButton> MouseDown;
    //public event Action<Mouse, MouseButton> MouseUp;
    //public event Action<Mouse, MouseButton> MouseClick;
    //public event Action<Mouse, MouseButton> MouseDoubleClick;
    //public event Action<Mouse, Vector2>     MouseScroll;

    public Mouse(IMouse mouse)
    {
        _silkMouse = mouse;

        _silkMouse.MouseMove   += OnMouseMove;
        _silkMouse.MouseDown   += OnMouseDown;
        _silkMouse.MouseUp     += OnMouseUp;
        _silkMouse.Click       += OnClick;
        _silkMouse.DoubleClick += OnDoubleClick;
        _silkMouse.Scroll      += OnScroll;

        PreviousPosition = Position = _silkMouse.Position;
        DoubleClickTime = 200;
    }

    public bool IsButtonPressed(MouseButton button) => _pressedButtons.Contains(button);

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        PreviousPosition = Position;
        Position = position;

        EventManager.RaiseEvent(EventTypeFlags.MouseMove, new EventRaiseDto { Sender = this, Device = this });
        //MouseMove?.Invoke(this, position);
    }

    private void OnMouseDown(IMouse mouse, Silk.NET.Input.MouseButton button)
    {
        var btn = (MouseButton)(int)button;
        _pressedButtons.Add(btn);

        EventManager.RaiseEvent(EventTypeFlags.MouseDown, new EventRaiseDto { Sender = this, Device = this, MouseButton = btn });
        //MouseDown?.Invoke(this, btn);
    }

    private void OnMouseUp(IMouse mouse, Silk.NET.Input.MouseButton button)
    {
        var btn = (MouseButton)(int)button;
        _pressedButtons.Remove(btn);

        EventManager.RaiseEvent(EventTypeFlags.MouseUp, new EventRaiseDto { Sender = this, Device = this, MouseButton = btn });
        //MouseUp?.Invoke(this, btn);
    }

    private void OnClick(IMouse mouse, Silk.NET.Input.MouseButton button, Vector2 vector)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseClick, new EventRaiseDto { Sender = this, Device = this, MouseButton = (MouseButton)(int)button });
        //MouseClick?.Invoke(this, (MouseButton)(int)button);
    }

    private void OnDoubleClick(IMouse mouse, Silk.NET.Input.MouseButton button, Vector2 vector)
    {
        EventManager.RaiseEvent(EventTypeFlags.MouseDoubleClick, new EventRaiseDto { Sender = this, Device = this, MouseButton = (MouseButton)(int)button });
        //MouseDoubleClick?.Invoke(this, (MouseButton)(int)button);
    }

    private void OnScroll(IMouse mouse, ScrollWheel wheel)
    {
        Scroll = new Vector2(wheel.X, wheel.Y);

        EventManager.RaiseEvent(EventTypeFlags.MouseScroll, new EventRaiseDto { Sender = this, Device = this });
        //MouseScroll?.Invoke(this, Scroll);
    }
}
