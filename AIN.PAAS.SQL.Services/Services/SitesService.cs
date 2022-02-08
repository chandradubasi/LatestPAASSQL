using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using AIN.PAAS.SQL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Services.Services
{
   public class SitesService : ISitesService
    {

        private readonly ISitesRepository _sitesRepository;
        public SitesService(ISitesRepository sitesRepository)
        {
            _sitesRepository = sitesRepository;
        }

        public async Task<SitesResponse> GetSite(string siteId)
        {
            return await _sitesRepository.GetSite(siteId);
        }

        public async Task<List<SitesResponse>> Getsites()
        {
            return await _sitesRepository.Getsites();
        }
    }
}
