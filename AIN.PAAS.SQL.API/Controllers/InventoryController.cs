using AIN.PAAS.SQL.Helper;
using AIN.PAAS.SQL.Helper.Constants;
using AIN.PAAS.SQL.Helper.Wrapper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Request;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.API.Controllers
{
    [Route(CommonConstants.Inventory.InventoryAPIControllerRoute)]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private IInventoryServices _inventoryServices;
        private readonly IUriService _uriService;
        private readonly AINDatabaseContext _aINDatabaseContext;

        public InventoryController(IInventoryServices inventoryServices, IUriService uriService, AINDatabaseContext aINDatabaseContext)
        {
            _inventoryServices = inventoryServices;
            _uriService = uriService;
            _aINDatabaseContext = aINDatabaseContext;
        }


        [HttpGet]
        [Route(CommonConstants.Inventory.GetInventoryById)]
        public async Task<IActionResult> Get(Guid inventoryId)
        {
            try
            {
                var inventoryItem = await _inventoryServices.GetInventoryDetailsById(inventoryId);

                if (inventoryItem == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    return new OkObjectResult(inventoryItem);
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpGet]
        [Route(CommonConstants.Inventory.GetInventoryByStatus)]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryByStatus(string status)
        {
            try
            {
                var inventorylist = await _inventoryServices.GetInventoryByStatus(status);
                if (inventorylist == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(inventorylist);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpPost]
        [Route(CommonConstants.Inventory.CreateInventoryRoute)]
        public async Task<IActionResult> InventoryCheckIn(CheckInRequest checkInRequest)
        {
            try
            {
                var checkinResponse = await _inventoryServices.InventoryCheckIn(checkInRequest);
                if (checkinResponse == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    return new OkObjectResult(checkinResponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpPut]
        [Route(CommonConstants.Inventory.CheckOutInventoryRoute)]
        public async Task<IActionResult> InventoryCheckOut(CheckOutRequest checkOutRequest)
        {
            try
            {
                var checkOutItem = await _inventoryServices.InventoryCheckOut(checkOutRequest);
                if (checkOutItem == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    return new OkObjectResult(StatusCodes.Status200OK);
                }
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpPut]
        [Route(CommonConstants.Inventory.InventoryTransfer)]
        public async Task<IActionResult> InventoryTransfer(TransferRequestData transferRequestData)
        {
            var transferResponse = await _inventoryServices.ItemTransfer(transferRequestData);
            try
            {
                if (transferResponse == null)
                {
                    return new BadRequestResult();
                }
                else
                {
                    return new OkObjectResult(transferResponse);
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

       
        [HttpGet]       
        [Route(CommonConstants.Inventory.GetInventoryByPaging)]
        public IActionResult GetInventorys([FromQuery] QueryStringParameters queryStringParameters)
        {
            try
            {
                var inventorylist = _inventoryServices.GetInventorys(queryStringParameters);

                var metadata = new
                {
                    inventorylist.TotalCount,
                    inventorylist.PageSize,
                    inventorylist.CurrentPage,
                    inventorylist.TotalPages,
                    inventorylist.HasNext,
                    inventorylist.HasPrevious
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));


                return Ok(inventorylist);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpGet]
        [Route(CommonConstants.Inventory.GetInventoryBySearch)]
        public async Task<IActionResult> GetInventoryBySearch([FromQuery] SearchInput  searchInput)
        {
            try
            {
                PaginationFilter paginationFilter = null;
                var route = Request.Path.Value;
                if (searchInput.PageNumber != 0 && searchInput.PageSize != 0)
                {
                    paginationFilter = new PaginationFilter(searchInput.PageNumber, searchInput.PageSize);
                }
                else
                {
                    paginationFilter = new PaginationFilter();
                    searchInput.PageNumber = 1;
                    searchInput.PageSize = 50;
                }
                var pagedData = await _inventoryServices.GetInventoryBySearch(searchInput);
                

                var totalRecords = pagedData.Count;
                var pagedReponse = PaginationHelper.CreatePagedReponse<InventoryResponse>(pagedData, paginationFilter, totalRecords, _uriService, route);
                return Ok(pagedReponse);

               
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpGet]
        [Route(CommonConstants.Inventory.GetInventoryByFilter)]
        public async Task<IActionResult> GetInventoryByFilter(Guid id, string fields)
        {
            try
            {
                
                var actualData = await _inventoryServices.GetInventoryByFilter(id);
                    if (actualData == null)
                    return NotFound();
                // Getting the fields is an expensive operation, so the default is all,
                // in which case we will just return the results
                if (!string.Equals(fields, "all", StringComparison.OrdinalIgnoreCase))
                    {
                        var shapedResults = actualData.Select(x => GetShapedObject(x, fields));                       
                    return Ok(shapedResults);
                }
                return Ok(actualData);

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryByPaging([FromQuery] PaginationFilter filter)
        {
            try
            {
                var route = Request.Path.Value;
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var pagedData = await _inventoryServices.GetInventoryByPaging(validFilter);

                var totalRecords = pagedData.Count;
                var pagedReponse = PaginationHelper.CreatePagedReponse<InventoryResponse>(pagedData, validFilter, totalRecords, _uriService, route);
                return Ok(pagedReponse);


            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, CommonConstants.APIConstant.InternalServerError);
            }
        }

        public object GetShapedObject<TParameter>(TParameter entity, string fields)
        {
            if (string.IsNullOrEmpty(fields))
                return entity;
            Regex regex = new Regex(@"[^,()]+(\([^()]*\))?");
            var requestedFields = regex.Matches(fields).Cast<Match>().Select(m => m.Value).Distinct();
            ExpandoObject expando = new ExpandoObject();

            foreach (var field in requestedFields)
            {
                if (field.Contains("("))
                {
                    var navField = field.Substring(0, field.IndexOf('('));

                    IList navFieldValue = entity.GetType()
                                                ?.GetProperty(navField, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                                                ?.GetValue(entity, null) as IList;
                    var regexMatch = Regex.Matches(field, @"\((.+?)\)");
                    if (regexMatch?.Count > 0)
                    {
                        var propertiesString = regexMatch[0].Value?.Replace("(", string.Empty).Replace(")", string.Empty);
                        if (!string.IsNullOrEmpty(propertiesString))
                        {
                            string[] navigationObjectProperties = propertiesString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            List<object> list = new List<object>();
                            foreach (var item in navFieldValue)
                            {
                                list.Add(GetShapedObject(item, navigationObjectProperties));
                            }

                            ((IDictionary<string, object>)expando).Add(navField, list);
                        }
                    }
                }
                else
                {
                    var value = entity.GetType()
                                      ?.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public)
                                      ?.GetValue(entity, null);
                    ((IDictionary<string, object>)expando).Add(field, value);
                }
            }

            return expando;
        }

        private object GetShapedObject<TParameter>(TParameter entity, IEnumerable<string> fields)
        {
            ExpandoObject expando = new ExpandoObject();
            foreach (var field in fields)
            {
                var value = entity.GetType()
                                  ?.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                                  ?.GetValue(entity, null);
                ((IDictionary<string, object>)expando).Add(field, value);
            }
            return expando;
        }
    }
}
