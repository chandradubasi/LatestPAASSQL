using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Repository.Repository
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly AINDatabaseContext _aINDatabaseContext;
        private readonly RegionConfigs _regionConfigs;
        public LocationsRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }

        public async Task<List<LocationsResponse>> GetLocations()
        {
            List<LocationsResponse> locationsList = new List<LocationsResponse>();
            try
            {
                var locations = await _aINDatabaseContext.Location.Include(s => s.Storage).ToListAsync();

                if (locations == null)
                {
                    return null;
                }
                else
                {
                    foreach (var location in locations)
                    {
                        LocationsResponse locationsResponse = new LocationsResponse()
                        {
                            LocationId = location.Id.ToString(),
                            Name = location.Name,   
                            Storages = location.Storage.Select(s => _regionConfigs.APIDomain + "/api/storages/" + s.Id).ToArray(),
                        };
                        locationsList.Add(locationsResponse);
                    }
                    return locationsList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<LocationsResponse> GetLocationsById(string locationId)
        {
            try
            {
                var locations = await _aINDatabaseContext.Location.Where(loc => loc.Id == Guid.Parse(locationId)).Include(s => s.Storage).FirstOrDefaultAsync();

                if (locations == null)
                {
                    return null;
                }
                else
                {
                    LocationsResponse locationsResponse = new LocationsResponse()
                    {
                        LocationId = locations.Id.ToString(),
                        Name = locations.Name,
                        Storages = locations.Storage.Select(s => _regionConfigs.APIDomain + "/api/storages/" + s.Id).ToArray(),
                    };
                    return locationsResponse;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
