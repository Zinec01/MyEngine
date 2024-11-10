namespace ECSEngineTest.Input;

public interface IInputDevice
{
    int Index { get; }
    string Name { get; }
    bool IsConnected { get; }
}
