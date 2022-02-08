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
    public class HospitalRepository : IHospitalRepository
    {
        private readonly AINDatabaseContext _aINDatabaseContext;
        private readonly RegionConfigs _regionConfigs;
        public HospitalRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }
        public async Task<List<HospitalsResponse>> GetHospital()
        {
            List<HospitalsResponse> hospitals = new List<HospitalsResponse>();
            try
            {
                var hospitalsList = await _aINDatabaseContext.Hospital.Include(h => h.Site).ToListAsync();
                if (hospitalsList != null)
                {
                    foreach (var hospital in hospitalsList)
                    {
                        HospitalsResponse hospitalsResponse = new HospitalsResponse()
                        {
                            HospitalId = hospital.Id.ToString(),
                            Name = hospital.Name,
                            Sites = hospital.Site.Select(e => _regionConfigs.APIDomain + "/api/sites/" + e.Id).ToArray()
                        };
                        hospitals.Add(hospitalsResponse);
                    }
                }
                return hospitals;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
