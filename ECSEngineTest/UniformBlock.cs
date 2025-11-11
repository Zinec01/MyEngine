namespace ECSEngineTest;

public readonly struct UniformBlock
{
    public readonly string Name;
    public readonly uint Binding;
    public readonly uint UBO;

    internal UniformBlock(string name, uint binding, uint ubo)
    {
        Name = name;
        Binding = binding;
        UBO = ubo;
    }
}
