using AIN.PAAS.SQL.Helper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Request;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIN.PAAS.SQL.Repository.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private static Random random = new Random();
        private readonly RegionConfigs _regionConfigs;
        private readonly AINDatabaseContext _aINDatabaseContext;
        public enum AvailableStatus
        {
            InTransit,
            Available,
            UnAvailable
        }


        public InventoryRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }

        public async Task<InventoryResponse> GetInventoryDetailsById(Guid inventoryId)
        {
            var inventoryItem = await _aINDatabaseContext.InventoryItem.Where(i => i.Id == inventoryId).Include(i => i.Product)
            .Include(i => i.Storage).FirstOrDefaultAsync();
            var inventoryItemDetails = new InventoryResponse
            {
                InventoryId = inventoryItem.Id.ToString(),
                SGTIN = inventoryItem.Sgtin,
                ProductId = _regionConfigs.APIDomain + "/api/products/" + inventoryItem.ProductId.ToString(),
                LotNumber = inventoryItem.LotNumber.ToString(),
                ExpiryDate = inventoryItem.ExpiryDate,
                StorageId = _regionConfigs.APIDomain + "/api/storages/" + inventoryItem.StorageId.ToString(),
            };
            return inventoryItemDetails;
        }
        public async Task<CheckInResponse> InventoryCheckIn(CheckInRequest checkInRequest)
        {
            var results = new InventoryItem();
            try
            {
                List<InventoryItem> dboInventoryItems = new List<InventoryItem>();
                if (checkInRequest.AutoId)
                {
                    checkInRequest.SGTIN = new string[checkInRequest.Quantity];
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    for (var i = 0; i < checkInRequest.Quantity; i++)
                    {
                        checkInRequest.SGTIN[i] = new string(Enumerable.Repeat(chars, 24)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                    }
                }
                foreach (var sgtin in checkInRequest.SGTIN)
                {
                    dboInventoryItems.Add(new InventoryItem()
                    {
                        Sgtin = sgtin,
                        StorageId = new Guid(checkInRequest.StorageId),
                        ProductId = new Guid(checkInRequest.ProductId),
                        ExpiryDate = checkInRequest.ExpiryDate,
                        LotNumber = Convert.ToInt32(checkInRequest.LotNumber),
                        Remarks = checkInRequest.Remarks,
                        Status = AvailableStatus.Available.ToString(),
                    });

                }

                _aINDatabaseContext.InventoryItem.AddRange(dboInventoryItems);
                await _aINDatabaseContext.SaveChangesAsync();

                CheckInResponse checkinResponse = new CheckInResponse()
                {
                    Status = "success",
                    SGTIN = checkInRequest.SGTIN,
                    Product = _regionConfigs.APIDomain + "/api/products/" + checkInRequest.ProductId,
                    ExpiryDate = checkInRequest.ExpiryDate,
                    Lot = checkInRequest.LotNumber,
                    Storage = _regionConfigs.APIDomain + "/api/Storages/" + checkInRequest.StorageId
                };

                return checkinResponse;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task<List<InventoryResponse>> GetInventoryByStatus(string availablestatus)
        {
            List<InventoryResponse> inventoryResponses = new List<InventoryResponse>();
            try
            {
                var inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.Status == availablestatus).ToListAsync();
                if (inventoryItems == null)
                {
                    return null;
                }
                foreach (var inventoryItem in inventoryItems)
                {
                    InventoryResponse inventoryResponse = new InventoryResponse()
                    {
                        InventoryId = inventoryItem.Id.ToString(),
                        SGTIN = inventoryItem.Sgtin,
                        ProductId = _regionConfigs.APIDomain + "/api/products/" + inventoryItem.ProductId.ToString(),
                        LotNumber = inventoryItem.LotNumber.ToString(),
                        ExpiryDate = inventoryItem.ExpiryDate,
                        StorageId = _regionConfigs.APIDomain + "/api/storages/" + inventoryItem.StorageId.ToString(),

                    };
                    inventoryResponses.Add(inventoryResponse);
                }
                return inventoryResponses;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<InventoryItem> InventoryCheckOut(CheckOutRequest checkOutRequest)
        {
            InventoryItem inventoryItem = new InventoryItem();
            try
            {
                for (var j = 0; j < checkOutRequest.SGTIN.Length; j++)
                {

                    inventoryItem = await _aINDatabaseContext.InventoryItem.Where(i => i.Sgtin == checkOutRequest.SGTIN[j].ToString()).FirstOrDefaultAsync();

                    if (inventoryItem == null)
                    {
                        return null;
                    }
                    else
                    {
                        inventoryItem.Status = AvailableStatus.UnAvailable.ToString();
                        await _aINDatabaseContext.SaveChangesAsync();
                    }
                }
                return inventoryItem;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<TransferRequestData> ItemTransfer(TransferRequestData transferRequest)
        {
            try
            {
                for (var j = 0; j < transferRequest.items.Length; j++)
                {
                    var transferItem = await _aINDatabaseContext.InventoryItem.Where(i => i.Id == new Guid(transferRequest.items[j]) && i.StorageId == new Guid(transferRequest.SourceId)).FirstOrDefaultAsync();
                    if (transferItem != null)
                    {

                        if (!(transferRequest.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase)) && !(transferRequest.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))) // update CheckInStatus with Pending approval
                        {

                            transferItem.Status = AvailableStatus.InTransit.ToString();
                            await _aINDatabaseContext.SaveChangesAsync();

                            TransferRequest dbtransferRequest = new TransferRequest()
                            {
                                Items = new Guid(transferRequest.items[j]),
                                SourceId = new Guid(transferRequest.SourceId),
                                DestinationId = new Guid(transferRequest.DestinationId),
                                Requester = transferRequest.Requester,
                                RequestedDate = transferRequest.RequestedDate,
                                Approver = transferRequest.Approver,
                                Status = transferRequest.Status,
                                Comments = transferRequest.Comments
                            };
                            _aINDatabaseContext.TransferRequest.AddRange(dbtransferRequest);
                            await _aINDatabaseContext.SaveChangesAsync();
                        }

                        else // approved or Rejected case
                        {
                            if (transferRequest.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                            {
                                transferItem.Status = AvailableStatus.Available.ToString();
                                transferItem.StorageId = new Guid(transferRequest.DestinationId);
                                await _aINDatabaseContext.SaveChangesAsync();

                                TransferRequest dbtransferRequest = new TransferRequest()
                                {
                                    Items = new Guid(transferRequest.items[j]),
                                    SourceId = new Guid(transferRequest.SourceId),
                                    DestinationId = new Guid(transferRequest.DestinationId),
                                    Requester = transferRequest.Requester,
                                    RequestedDate = transferRequest.RequestedDate,
                                    Approver = transferRequest.Approver,
                                    Status = transferRequest.Status,
                                    Comments = transferRequest.Comments
                                };
                                _aINDatabaseContext.TransferRequest.AddRange(dbtransferRequest);
                                await _aINDatabaseContext.SaveChangesAsync();
                            }

                            else if (transferRequest.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                            {
                                transferItem.Status = AvailableStatus.Available.ToString();
                                await _aINDatabaseContext.SaveChangesAsync();

                                TransferRequest dbtransferRequest = new TransferRequest()
                                {
                                    Items = new Guid(transferRequest.items[j]),
                                    SourceId = new Guid(transferRequest.SourceId),
                                    DestinationId = new Guid(transferRequest.DestinationId),
                                    Requester = transferRequest.Requester,
                                    RequestedDate = transferRequest.RequestedDate,
                                    Approver = transferRequest.Approver,
                                    Status = transferRequest.Status,
                                    Comments = transferRequest.Comments
                                };

                                _aINDatabaseContext.TransferRequest.AddRange(dbtransferRequest);
                                await _aINDatabaseContext.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return transferRequest;
        }

        public PagedList<InventoryItem> GetInventorys(QueryStringParameters queryStringParameters)
        {

            try
            {
                IQueryable<InventoryItem> inventoryItems = _aINDatabaseContext.InventoryItem.AsQueryable();

                return PagedList<InventoryItem>.ToPagedList(inventoryItems,
                queryStringParameters.PageNumber,
                queryStringParameters.PageSize);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<InventoryResponse>> GetInventoryByPaging(PaginationFilter filter)
        {
            List<InventoryResponse> inventoryResponses = new List<InventoryResponse>();
            try
            {
                var pagedData = await _aINDatabaseContext.InventoryItem
                .Skip((filter.PageNumber - 1) * filter.PageSize)
              .Take(filter.PageSize).ToListAsync();
                if (pagedData == null)
                {
                    return null;
                }
                foreach (var inventoryItem in pagedData)
                {
                    InventoryResponse inventoryResponse = new InventoryResponse()
                    {
                        InventoryId = inventoryItem.Id.ToString(),
                        SGTIN = inventoryItem.Sgtin,
                        ProductId = _regionConfigs.APIDomain + "/api/products/" + inventoryItem.ProductId.ToString(),
                        LotNumber = inventoryItem.LotNumber.ToString(),
                        ExpiryDate = inventoryItem.ExpiryDate,
                        StorageId = _regionConfigs.APIDomain + "/api/storages/" + inventoryItem.StorageId.ToString(),

                    };
                    inventoryResponses.Add(inventoryResponse);
                }
                return inventoryResponses;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<InventoryResponse>> GetInventoryBySearch(SearchInput searchInput)
        {
            List<InventoryResponse> inventoryResponses = new List<InventoryResponse>();
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            try
            {
                inventoryItems = await GetDataBySearchCombination(searchInput);


                if (inventoryItems == null)
                {
                    return null;
                }
                foreach (var inventoryItem in inventoryItems)
                {
                    InventoryResponse inventoryResponse = new InventoryResponse()
                    {
                        InventoryId = inventoryItem.Id.ToString(),
                        SGTIN = inventoryItem.Sgtin,
                        ProductId = _regionConfigs.APIDomain + "/api/products/" + inventoryItem.ProductId.ToString(),
                        LotNumber = inventoryItem.LotNumber.ToString(),
                        ExpiryDate = inventoryItem.ExpiryDate,
                        StorageId = _regionConfigs.APIDomain + "/api/storages/" + inventoryItem.StorageId.ToString(),

                    };
                    inventoryResponses.Add(inventoryResponse);
                }
                return inventoryResponses;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<InventoryItem>> GetInventoryByFilter(Guid inventoryId)
        {
            var results = await _aINDatabaseContext.InventoryItem.Where(i => i.Id == inventoryId).Include(i => i.Product)
            .Include(i => i.Storage).ToListAsync();

            return results;
        }

        public async Task<List<InventoryItem>> GetDataBySearchCombination(SearchInput searchInput)
        {

            List<InventoryItem> inventoryItems = new List<InventoryItem>();

            if (searchInput.ProductId != null && searchInput.Sgtin != null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.StorageId == null && searchInput.Sgtin == null && searchInput.StorageId == null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();

            }
            else if (searchInput.ProductId != null && searchInput.Sgtin == null && searchInput.StorageId == null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId != null && searchInput.Sgtin != null && searchInput.StorageId == null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId != null && searchInput.Sgtin == null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId == null && searchInput.Sgtin != null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId != null && searchInput.Sgtin != null && searchInput.StorageId == null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.ProductId == searchInput.ProductId && i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId == null && searchInput.Sgtin != null && searchInput.StorageId == null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId == null && searchInput.Sgtin == null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId == null && searchInput.Sgtin != null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.Sgtin == searchInput.Sgtin).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }
            else if (searchInput.ProductId != null && searchInput.Sgtin == null && searchInput.StorageId != null)
            {
                if (searchInput.SortByExpiryDate == null)
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "asc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).OrderBy(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
                else if (searchInput.SortByExpiryDate.ToLower() == "desc")
                    inventoryItems = await _aINDatabaseContext.InventoryItem.Where(i => i.StorageId == searchInput.StorageId && i.ProductId == searchInput.ProductId).OrderByDescending(o => o.ExpiryDate).Skip((searchInput.PageNumber - 1) * searchInput.PageSize).Take(searchInput.PageSize).ToListAsync();
            }

            return inventoryItems;
        }   
    }
}
