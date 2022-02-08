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
   public class StorageServices : IStorageServices
    {
        private IStorageRepository _storageRepository;
        public StorageServices(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;

        }       

        public async Task<List<StoragesResponse>> GetStorages()
        {
            return await _storageRepository.GetStorages();
        }

        public async Task<StoragesResponse> GetStoragesById(string storageId)
        {
            return await _storageRepository.GetStoragesById(storageId);
        }
    }
}
