namespace FileSoftware.Models.Responses
{
    public class FileUploadResponse
    {
        public FileUploadResponse()
        {
            this.FileUploads = new List<FileUploadResponseItem>();
        }

        public string Message { get; set; }

        public bool HasConflictResponse => (FileUploads.Any(x => x.Status) && FileUploads.Any(x => !x.Status));

        public bool HasFailResponse => FileUploads.All(x => !x.Status);


        public IList<FileUploadResponseItem> FileUploads { get; set; }

        public static FileUploadResponse CommonResponse(string message = null) => new FileUploadResponse
        {
            Message = message,
        };
    }
}
