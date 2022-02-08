using AIN.PAAS.SQL.Helper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Request;
using AIN.PAAS.SQL.Models.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Repository.IRepository
{
    public interface IInventoryRepository
    {
        Task<InventoryResponse> GetInventoryDetailsById(Guid inventoryId);
        Task<CheckInResponse> InventoryCheckIn(CheckInRequest checkInRequest);
        Task<List<InventoryResponse>> GetInventoryByStatus(string status);
        Task<InventoryItem> InventoryCheckOut(CheckOutRequest checkOutRequest);
        Task<TransferRequestData> ItemTransfer(TransferRequestData transferRequestData);
        PagedList<InventoryItem> GetInventorys(QueryStringParameters queryStringParameters);
        Task<List<InventoryResponse>> GetInventoryByPaging(PaginationFilter filter);
        Task<List<InventoryResponse>> GetInventoryBySearch(SearchInput searchInput);
        Task<List<InventoryItem>> GetInventoryByFilter(Guid id);
    }
}
