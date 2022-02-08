using AIN.PAAS.SQL.Models.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Repository.IRepository
{
    public interface ILocationsRepository
    {
        Task<LocationsResponse> GetLocationsById(string locationId);
        Task<List<LocationsResponse>> GetLocations();
    }
}
