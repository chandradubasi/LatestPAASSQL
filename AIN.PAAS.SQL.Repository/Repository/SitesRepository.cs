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
    public class SitesRepository : ISitesRepository
    {

        private readonly AINDatabaseContext _aINDatabaseContext;
        private readonly RegionConfigs _regionConfigs;
        public SitesRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }

        public async Task<SitesResponse> GetSite(string siteId)
        {
            try
            {
                var sitelist = await _aINDatabaseContext.Site.Where(s => s.Id == Guid.Parse(siteId)).Include(h => h.Lab).FirstOrDefaultAsync();

                if (sitelist == null)
                {
                    return null;
                }
                else
                {
                    SitesResponse sitesResponse = new SitesResponse()
                    {
                        SiteId = sitelist.Id.ToString(),
                        Name = sitelist.Name,
                        Labs = sitelist.Lab.Select(e => _regionConfigs.APIDomain + "/api/labs/" + e.Id).ToArray(),
                    };
                    return sitesResponse;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<SitesResponse>> Getsites()
        {
            List<SitesResponse> sites = new List<SitesResponse>();
            try
            {
                var sitelist = await _aINDatabaseContext.Site.Include(h => h.Lab).ToListAsync();

                if (sitelist == null)
                {
                    return null;
                }
                else
                {
                    foreach (var site in sitelist)
                    {
                        SitesResponse sitesResponse = new SitesResponse()
                        {
                            SiteId = site.Id.ToString(),
                            Name = site.Name,
                            Labs = site.Lab.Select(e => _regionConfigs.APIDomain + "/api/labs/" + e.Id).ToArray(),
                        };
                        sites.Add(sitesResponse);
                    }

                    return sites;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
