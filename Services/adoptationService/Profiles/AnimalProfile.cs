using AutoMapper;
using AnimalCatalog.API.DTOs.Requests;
using AnimalCatalog.API.DTOs.Responses;
using AnimalCatalog.API.Entities;

namespace AnimalCatalog.API.Profiles
{
    public class AnimalProfile : Profile
    {
        public AnimalProfile()
        {
            CreateMap<AnimalRequest, Animal>();

            CreateMap<Animal, AnimalResponse>()
                .ForMember(dest => dest.Sex, opt => opt.MapFrom(src => src.Sex.ToString()))
                .ForMember(dest => dest.Species, opt => opt.MapFrom(src => src.Species.ToString()))
                .ForMember(dest => dest.PetSize, opt => opt.MapFrom(src => src.PetSize.ToString()));
        }
    }
}