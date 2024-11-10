using ECSEngineTest.Input;

namespace ECSEngineTest;

internal class EventRaiseDto
{
    public object? Sender { get; set; }
    public IInputDevice Device { get; set; }
    public MouseButton MouseButton { get; set; }
    public Key Key { get; set; }
    public char Char { get; set; }
    public Window Window { get; set; }
    public string[] FilePaths { get; set; }
}
