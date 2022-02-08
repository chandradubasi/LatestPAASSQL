using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using AIN.PAAS.SQL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Services.Services
{
   public class LocationsServices : ILocationsServices
    {
        private ILocationsRepository _locationsRepository;

        public LocationsServices(ILocationsRepository locationsRepository)
        {
            _locationsRepository = locationsRepository;
        }

        public async Task<List<LocationsResponse>> GetLocations()
        {
            return await _locationsRepository.GetLocations();
        }

        public async Task<LocationsResponse> GetLocationsById(string locationId)
        {
            return await _locationsRepository.GetLocationsById(locationId);
        }

        
    }
}
