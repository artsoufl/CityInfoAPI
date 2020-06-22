using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Models.CityWithoutPointsOfInterestDto, Entities.City>();
            CreateMap<Entities.City, Models.CityDto>().ReverseMap();
        }
    }
}
