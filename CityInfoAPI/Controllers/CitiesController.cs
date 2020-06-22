using AutoMapper;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityRepo;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityRepo, IMapper mapper)
        {
            _cityRepo = cityRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cityEntities = _cityRepo.GetCities();
            
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePoi = false)
        {
            var city = _cityRepo.GetCity(id, includePoi);
            if (city == null) return NotFound();

            if (includePoi)
            {
                return Ok(_mapper.Map<CityDto>(city));
                //return Ok(_mapper.Map<IEnumerable<CityDto>>(city));
            }
            else
            {
                return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
                //return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(city));
            }
        }
    }

}
