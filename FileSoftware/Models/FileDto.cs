using System.ComponentModel.DataAnnotations;

namespace FileSoftware.Models
{
    public class FileDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string Extension { get; set; }

        public string FullName => $"{Name}.{Extension}";

        public string FilePath { get; set; }

        public string UploadedOn { get; set; }
    }
}
