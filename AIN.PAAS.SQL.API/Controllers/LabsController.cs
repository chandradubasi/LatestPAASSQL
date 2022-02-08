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
    [Route("api/[controller]")]
    [ApiController]
    public class LabsController : ControllerBase
    {

        private readonly ILabsService _labsService;

        public LabsController(ILabsService labsService)
        {
            _labsService = labsService;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LabsResponse>> Get()
        {
            try
            {
                var labs = await _labsService.GetLabs();
                if (labs != null)
                {
                    return new OkObjectResult(labs);
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

        [HttpGet()]
        [Route(CommonConstants.Lab.GetLabById)]
        public async Task<ActionResult<LabsResponse>> Get(string labId)
        {
            try
            {
                var labsResponseList = await _labsService.GetLab(labId);
                if (labsResponseList != null)
                {
                    return new OkObjectResult(labsResponseList);
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
