using System.ComponentModel.DataAnnotations;

namespace FileSoftware.Data.Entities
{
    public class FileEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Extension { get; set; }

        [Required]
        public Guid UniqueIdentifier { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public long Size { get; set; }


        public DateTime UploadedOn { get; set; }
    }
}
