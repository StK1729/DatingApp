using System.Linq;
using AutoMapper;
using DatingApp.api.Dtos;
using DatingApp.api.Models;

namespace DatingApp.api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(destination=> destination.PhotoUrl, options=> {
                    options.MapFrom(source=>source.Photos.FirstOrDefault(photo=>photo.IsMain).Url);
                })
                .ForMember(destination=> destination.Age, options=> {
                    options.MapFrom(d=> d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
                .ForMember(destination=> destination.PhotoUrl, options=>{
                    options.MapFrom(source=> source.Photos.FirstOrDefault(photo=> photo.IsMain).Url);
                })
                .ForMember(destination=> destination.Age, options => {
                    options.MapFrom(d=> d.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDatailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
        }
    }
}