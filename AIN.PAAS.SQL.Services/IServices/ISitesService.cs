using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Services.IServices
{
    public interface ISitesService
    {
        Task<SitesResponse> GetSite(string siteId);
        Task<List<SitesResponse>> Getsites();
    }
}
