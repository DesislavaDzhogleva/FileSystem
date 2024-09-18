using AutoMapper;
using FileSoftware.Data.Entities;

namespace FileSoftware.Models.Profiles
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<FileEntity, FileInputModel>()
                 .ForMember(dest =>
                    dest.UploadedOn,
                    opt => opt.MapFrom(src => src.UploadedOn.ToString("yyyy-MM-dd HH:mm:ss")));
        }
    }
}
