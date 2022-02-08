using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Repository.IRepository
{
    public interface IStorageRepository
    {
        Task<List<StoragesResponse>> GetStorages();
        Task<StoragesResponse> GetStoragesById(string storageId);
    }
}
