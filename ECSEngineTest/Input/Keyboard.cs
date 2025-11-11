using Silk.NET.Input;

namespace ECSEngineTest.Input;

public class Keyboard : IInputDevice
{
    private readonly IKeyboard _silkKeyboard;
    private readonly List<Key> _pressedKeys = [];

    public IReadOnlyList<Key> PressedKeys => _pressedKeys;
    public int Index => _silkKeyboard.Index;
    public string Name => _silkKeyboard.Name;
    public bool IsConnected => _silkKeyboard.IsConnected;
    public string Clipboard => _silkKeyboard.ClipboardText;

    //public event Action<Keyboard, Key> KeyDown;
    //public event Action<Keyboard, Key> KeyUp;
    //public event Action<Keyboard, char> KeyChar;

    public Keyboard(IKeyboard keyboard)
    {
        _silkKeyboard = keyboard;

        _silkKeyboard.KeyDown += OnKeyDown;
        _silkKeyboard.KeyUp   += OnKeyUp;
        _silkKeyboard.KeyChar += OnKeyChar;
    }

    public bool IsKeyPressed(Key key) => _pressedKeys.Contains(key);

    private void OnKeyDown(IKeyboard keyboard, Silk.NET.Input.Key key, int arg3)
    {
        var k = (Key)(int)key;
        _pressedKeys.Add(k);

        EventManager.RaiseEvent(EventTypeFlags.KeyDown, new EventRaiseDto { Sender = this, Device = this, Key = k });
        //KeyDown?.Invoke(this, k);
    }

    private void OnKeyUp(IKeyboard keyboard, Silk.NET.Input.Key key, int arg3)
    {
        var k = (Key)(int)key;
        _pressedKeys.Remove(k);

        EventManager.RaiseEvent(EventTypeFlags.KeyUp, new EventRaiseDto { Sender = this, Device = this, Key = k });
        //KeyUp?.Invoke(this, k);
    }

    private void OnKeyChar(IKeyboard keyboard, char ch)
    {
        EventManager.RaiseEvent(EventTypeFlags.KeyChar, new EventRaiseDto { Sender = this, Device = this, Char = ch });
        //KeyChar?.Invoke(this, ch);
    }
}
