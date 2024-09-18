namespace FileSoftware.Constants
{
    public static class FileUploadMessages
    {

        public const string NoFilesProvided = "No files were provided.";

        public const string CommonSuccess = $"Files have been uploaded successfully.";

        public const string MixedSuccess = $"Some of the files have been uploaded successfully.";

        public const string CommonFail = $"Files have not been uploaded.";

        public static string FileIsEmpty(string name) =>
           $"A file with the name '{name}' is empty.";

        public static string FileAlreadyExists(string name, string extension) =>
            $"A file with the name '{name}' and extension '{extension}' already exists.";

        public static string FileUploadedSuccessfully(string name, string extension) =>
         $"A file with the name '{name}' and extension '{extension}' has been uploaded.";
    }

}
