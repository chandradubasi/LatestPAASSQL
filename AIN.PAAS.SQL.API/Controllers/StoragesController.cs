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
    [Route(CommonConstants.Storage.LocationsAPIControllerRoute)]
    [ApiController]
    public class StoragesController : ControllerBase
    {
        private IStorageServices _storageServices;
        public StoragesController(IStorageServices storageServices)
        {
            _storageServices = storageServices;
        }

     

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<StoragesResponse>> Get()
        {
            try
            {
                var storages = await _storageServices.GetStorages();
                if (storages != null)
                {
                    return new OkObjectResult(storages);
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
        [Route(CommonConstants.Storage.GetStorageById)]
        public async Task<ActionResult<StoragesResponse>> Get(string storageId)
        {
            try
            {
                var storageResponse = await _storageServices.GetStoragesById(storageId);

                if (storageResponse != null)
                {
                    return new OkObjectResult(storageResponse);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

    }
}
