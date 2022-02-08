using AIN.PAAS.SQL.Helper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Request;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using AIN.PAAS.SQL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Services.Services
{
    public class InventoryServices : IInventoryServices
    {
        private IInventoryRepository _inventoryRepository;
        public InventoryServices(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }
        public async Task<InventoryResponse> GetInventoryDetailsById(Guid inventoryId)
        {
            return await _inventoryRepository.GetInventoryDetailsById(inventoryId);
        }
        public async Task<List<InventoryResponse>> GetInventoryByStatus(string status)
        {
            return await _inventoryRepository.GetInventoryByStatus(status);
        }

        public async Task<CheckInResponse> InventoryCheckIn(CheckInRequest checkInRequest)
        {
            return await _inventoryRepository.InventoryCheckIn(checkInRequest);
        }

        public async Task<InventoryItem> InventoryCheckOut(CheckOutRequest checkOutRequest)
        {
            return await _inventoryRepository.InventoryCheckOut(checkOutRequest);
        }

        public async Task<TransferRequestData> ItemTransfer(TransferRequestData transferRequestData)
        {
            return await _inventoryRepository.ItemTransfer(transferRequestData);
        }

        public PagedList<InventoryItem> GetInventorys(QueryStringParameters queryStringParameters)
        {
            return  _inventoryRepository.GetInventorys(queryStringParameters);
        }

        public Task<List<InventoryResponse>> GetInventoryByPaging(PaginationFilter filter)
        {
            return _inventoryRepository.GetInventoryByPaging(filter);
        }

        public Task<List<InventoryResponse>> GetInventoryBySearch(SearchInput searchInput)
        {
            return _inventoryRepository.GetInventoryBySearch(searchInput);
        }

        public Task<List<InventoryItem>> GetInventoryByFilter(Guid id)
        {
            return _inventoryRepository.GetInventoryByFilter(id);
        }
    }
}
