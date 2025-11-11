using Silk.NET.Input;

namespace ECSEngineTest.Input;

public static class InputManager
{
    private static IInputContext _inputContext;
    private static readonly Dictionary<IMouse, Mouse> _mice = [];
    private static readonly Dictionary<IKeyboard, Keyboard> _keyboards = [];

    public static IReadOnlyCollection<Mouse> Mice => _mice.Values;
    public static IReadOnlyCollection<Keyboard> Keyboards => _keyboards.Values;

    public static void Init(IInputContext inputContext)
    {
        _inputContext = inputContext;

        _inputContext.ConnectionChanged += OnConnectionChanged;

        SetupMice();
        SetupKeyboards();
        SetupGamepads();
    }

    private static void SetupMice()
    {
        foreach (var mouse in _inputContext.Mice)
        {
            _mice.Add(mouse, new Mouse(mouse));
        }
    }

    private static void SetupKeyboards()
    {
        foreach (var keyboard in _inputContext.Keyboards)
        {
            _keyboards.Add(keyboard, new Keyboard(keyboard));
        }
    }

    private static void SetupGamepads()
    {
        //TODO: Add gamepad support
    }

    private static void OnConnectionChanged(Silk.NET.Input.IInputDevice device, bool isConnected)
    {
        if (device is IMouse silkMouse)
        {
            if (!_mice.TryGetValue(silkMouse, out var mouse))
            {
                _mice.Add(silkMouse, mouse = new Mouse(silkMouse));
            }
            
            EventManager.RaiseEvent(EventTypeFlags.InputDeviceConnectionChanged, new EventRaiseDto { Device = mouse });
        }
        else if (device is IKeyboard silkKeyboard)
        {
            if (!_keyboards.TryGetValue(silkKeyboard, out var keyboard))
            {
                _keyboards.Add(silkKeyboard, keyboard = new Keyboard(silkKeyboard));
            }
            
            EventManager.RaiseEvent(EventTypeFlags.InputDeviceConnectionChanged, new EventRaiseDto { Device = keyboard });
        }
    }
}
