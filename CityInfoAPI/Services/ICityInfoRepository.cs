using CityInfoAPI.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Services
{
    public interface ICityInfoRepository
    {

        IEnumerable<City> GetCities();

        City GetCity(int cityId, bool includePoi);

        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);

        bool CityExists(int cityId);

        void AddPointOfInterestForCity(int cityId, PointOfInterest poi);

        bool Save();

        void UpdatePointOfInterestForCity(int cityId, PointOfInterest poi)
    }
}
