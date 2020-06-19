using CityInfoAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poi = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            if (poi == null) return NotFound();

            return Ok(poi);
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityid, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null) return NotFound();

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p =>p.Id);

            var finalPoi = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPoi);

            // validating the input that way
            return CreatedAtRoute(
                "GetPointOfInterest", 
                new { cityid = cityid, id = finalPoi.Id},
                finalPoi);
        }

        [HttpPut("{id}")]    // id of the poi that we will update
        public IActionResult UpdatePointOfInterest(int cityid, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest == null) return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null) return NotFound();

            var poiFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poiFromStore == null) return NotFound();

            // in this way if in the new object description is null then it will be set to null
            poiFromStore.Name = pointOfInterest.Name;
            poiFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        // we need this in order to be able to update specific properties and not the whole object
        // so we can update only the name or only the description
        [HttpPatch("{id}")]    // id of the poi that we will update
        public IActionResult PartiallyUpdatePointOfInterest(int cityid, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);
            if (city == null) return NotFound();

            var poiFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poiFromStore == null) return NotFound();

            var pointOfInterestToPatch =
                new PointOfInterestForUpdateDto()
                {
                    Name = poiFromStore.Name,
                    Description = poiFromStore.Description
                };

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "Description should be different from the name");
            }

            if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

            poiFromStore.Name = pointOfInterestToPatch.Name;
            poiFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null) return NotFound();

            var poiFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (poiFromStore == null) return NotFound();

            city.PointsOfInterest.Remove(poiFromStore);

            return NoContent();
        }

    }
}
