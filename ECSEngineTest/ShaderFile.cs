namespace ECSEngineTest;

public struct ShaderFile(string filePath, ShaderType type)
{
    public string FilePath = filePath;
    public ShaderType Type = type;
}
