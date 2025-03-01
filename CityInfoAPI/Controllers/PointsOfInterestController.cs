﻿using AutoMapper;
using CityInfoAPI.Entities;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityRepo;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mail, ICityInfoRepository cityRepo, IMapper mapper)
        {
            _logger = logger;
            _mailService = mail;
            _cityRepo = cityRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityRepo.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found");
                    return NotFound();
                }

                var poiForCity = _cityRepo.GetPointsOfInterest(cityId);             
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(poiForCity));
            }
            catch (Exception e)
            {
                _logger.LogCritical($"City with id {cityId} throw an error when accessing points of interest " + e.ToString());
                return StatusCode(500, "A problem happened while handling the request");
            }
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityRepo.CityExists(cityId))
            {
                _logger.LogInformation($"City with id {cityId} was not found");
                return NotFound();
            }

            var poi = _cityRepo.GetPointOfInterestForCity(cityId, id);
            if (poi == null) return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(poi));
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityid, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();

            if (!_cityRepo.CityExists(cityid)) return NotFound();

            var finalPoi = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityRepo.AddPointOfInterestForCity(cityid, finalPoi);
            _cityRepo.Save();

            var createdPoi = _mapper.Map<Models.PointOfInterestDto>(finalPoi);

            // validating the input that way
            return CreatedAtRoute(
                "GetPointOfInterest", 
                new { cityid = cityid, id = createdPoi.Id},
                createdPoi);
        }

        [HttpPut("{id}")]    // id of the poi that we will update
        public IActionResult UpdatePointOfInterest(int cityid, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();

            if (!_cityRepo.CityExists(cityid)) return NotFound();

            var poiEntity = _cityRepo.GetPointOfInterestForCity(cityid, id);
            if (poiEntity == null) return NotFound();

            _mapper.Map(pointOfInterest, poiEntity);

            _cityRepo.UpdatePointOfInterestForCity(cityid, poiEntity);
            _cityRepo.Save();

            return NoContent();
        }

        // we need this in order to be able to update specific properties and not the whole object
        // so we can update only the name or only the description
        [HttpPatch("{id}")]    // id of the poi that we will update
        public IActionResult PartiallyUpdatePointOfInterest(int cityid, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (!_cityRepo.CityExists(cityid)) return NotFound();

            var poiEntity = _cityRepo.GetPointOfInterestForCity(cityid, id);
            if (poiEntity == null) return NotFound();

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(poiEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "Description should be different from the name");
            }

            if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

            _mapper.Map(pointOfInterestToPatch, poiEntity);

            _cityRepo.UpdatePointOfInterestForCity(cityid, poiEntity);
            _cityRepo.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityRepo.CityExists(cityId)) return NotFound();

            var poiEntity = _cityRepo.GetPointOfInterestForCity(cityId, id);
            if (poiEntity == null) return NotFound();

            _cityRepo.DeletePointOfInterest(poiEntity);
            _cityRepo.Save();

            _mailService.Send("Point of interest deleted", $"Point of interest {poiEntity.Name} with id {poiEntity.Id}");

            return NoContent();
        }

    }
}
