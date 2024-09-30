namespace ECSEngineTest;

public readonly struct ShaderFile(string filePath, ShaderType type)
{
    public readonly string FilePath = filePath;
    public readonly ShaderType Type = type;
}
