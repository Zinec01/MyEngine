namespace ECSEngineTest;

public struct Interpolatable<T> where T : unmanaged
{
    public T Previous { get; set; } = default;
    public T Current { get; set; } = default;
    public T Target { get; set; } = default;

    public Interpolatable() { }

    public Interpolatable(T initialValue) : this()
    {
        Current = initialValue;
    }
}
