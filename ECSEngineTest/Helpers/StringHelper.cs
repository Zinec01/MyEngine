namespace ECSEngineTest.Helpers
{
    public static class StringHelper
    {
        public static bool ValidateFilePath(ref string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                //throw new ArgumentNullException(nameof(filePath));
                return false;

            if (!Path.IsPathFullyQualified(filePath))
                filePath = Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
                //throw new FileNotFoundException(null, filePath);
                return false;

            return true;
        }
    }
}
