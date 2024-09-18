namespace FileSoftware.Constants
{
    public static class FileConstants
    {
        public static string StoragePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public static string FileNameWithoutIdentifier(string name, string extension) => $"{name}.{extension}";

        public static string FileNameWithIdentifier(string name, string extension, string identifier) => $"{name}_{identifier}.{extension}";
    }
}
