using System;
using System.Linq;
using AutoMapper;
using DTApp.API.DTO;
using DTApp.API.Models;

namespace DTApp.API.Helper
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,  UserForListDto>().ForMember(
                dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url);
                }).ForMember(
                    dest => dest.Age, opt => {
                        opt.MapFrom(src => (DateTime.Now.Year - src.DateOfBirth.Year));
                    }
                );
            CreateMap<User,  UserForDetailedDto>().ForMember(
                dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url);
                }).ForMember(
                    dest => dest.Age, opt => {
                        opt.MapFrom(src => (DateTime.Now.Year - src.DateOfBirth.Year));
                    }
                );
            CreateMap<Photo,  PhotoDto>();

            CreateMap<UserForUpdateDTO, User>();
        }
    }
}