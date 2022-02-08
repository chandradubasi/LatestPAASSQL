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
    public class LabsRepository : ILabsRepository
    {

        private readonly AINDatabaseContext _aINDatabaseContext;
        private readonly RegionConfigs _regionConfigs;
        public LabsRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }

        public async Task<LabsResponse> GetLab(string labID)
        {
            try
            {
                var labs = await _aINDatabaseContext.Lab.Where(l => l.Id == Guid.Parse(labID)).Include(loc => loc.Location).FirstOrDefaultAsync();

                if (labs == null)
                {
                    return null;
                }
                else
                {
                    LabsResponse labsResponse = new LabsResponse()
                    {
                        LabId = labs.Id.ToString(),
                        Name = labs.Name,
                        Locations = labs.Location.Select(loc => _regionConfigs.APIDomain + "/api/locations/" + loc.Id).ToArray(),
                    };
                    return labsResponse;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<LabsResponse>> GetLabs()
        {
            List<LabsResponse> labsList = new List<LabsResponse>();
            try
            {
                var labs = await _aINDatabaseContext.Lab.Include(loc => loc.Location).ToListAsync();

                if (labs == null)
                {
                    return null;
                }
                else
                {
                    foreach (var lab in labs)
                    {
                        LabsResponse labsResponse = new LabsResponse()
                        {
                            LabId = lab.Id.ToString(),
                            Name = lab.Name,
                            Locations = lab.Location.Select(loc => _regionConfigs.APIDomain + "/api/locations/" + loc.Id).ToArray(),
                        };

                        labsList.Add(labsResponse);
                    }

                    return labsList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
