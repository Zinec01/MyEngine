namespace ECSEngineTest.Helpers;

internal static class FileHelper
{
    private static readonly string[] _supportedModelExtensions = [".obj", ".fbx", ".gltf", ".glb"];

    public static bool ValidateFilePath(ref string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        if (!Path.IsPathFullyQualified(filePath))
            filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
            return false;

        return true;
    }

    public static bool IsSupportedModelFile(string filePath)
    {
        return _supportedModelExtensions.Contains(Path.GetExtension(filePath));
    }
}
