using AIN.PAAS.SQL.Helper.Constants;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.API.Controllers
{
    [Route(CommonConstants.Site.SiteAPIControllerRoute)]
    [ApiController]
    public class SitesController : ControllerBase
    {
        private readonly ISitesService _sitesService;
        public SitesController(ISitesService sitesService)
        {
            _sitesService = sitesService;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<SitesResponse>> Get()
        {
            try
            {
                var sites = await _sitesService.Getsites();
                if (sites != null)
                {
                    return new OkObjectResult(sites);
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpGet]
        [Route(CommonConstants.Site.GetSiteById)]
        public async Task<ActionResult<SitesResponse>> Get(string siteId)
        {
            try
            {
                var sitesResponseList = await _sitesService.GetSite(siteId);
                if (sitesResponseList != null)
                {
                    return new OkObjectResult(sitesResponseList);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }
    }
}
