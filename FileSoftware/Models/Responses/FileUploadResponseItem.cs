namespace FileSoftware.Models.Responses
{
    public class FileUploadResponseItem
    {
        public string FileName { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; }

        public static FileUploadResponseItem UploadFail(string message, string fileName) => new FileUploadResponseItem
        {
            Status = false,
            Message = message,
            FileName = fileName
        };

        public static FileUploadResponseItem UploadSuccess(string message, string fileName) => new FileUploadResponseItem
        {
            Status = true,
            Message = message,
            FileName = fileName
        };
    }
}
