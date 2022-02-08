using AIN.PAAS.SQL.Helper.Constants;
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
    [Route(CommonConstants.Location.LocationsAPIControllerRoute)]
    [ApiController]
    public class LocationsController : ControllerBase
    {

        private ILocationsServices _locationsServices;

        public LocationsController(ILocationsServices locationsServices)
        {
            _locationsServices = locationsServices;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LocationsResponse>> Get()
        {
            try
            {
                var locationResponse = await _locationsServices.GetLocations();
                if (locationResponse != null)
                {
                    return new OkObjectResult(locationResponse);
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
        [HttpGet]
        [Route(CommonConstants.Location.GetLocationById)]
        public async Task<ActionResult<LocationsResponse>> GetById(string locationId)
        {
            try
            {
                var locationResponse = await _locationsServices.GetLocationsById(locationId);
                if (locationResponse != null)
                {
                    return new OkObjectResult(locationResponse);
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
