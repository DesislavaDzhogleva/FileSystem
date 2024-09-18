namespace FileSoftware.Models
{
    public class FileInfoModel
    {
        public FileInfoModel(bool fileAlreadyExists, string name, string extensions)
        {
            FileAlreadyExists = fileAlreadyExists;
            Name = name;
            Extensions = extensions;
        }

        public bool FileAlreadyExists { get; set; }

        public string Name { get; set; }

        public string Extensions { get; set; }
    }
}
