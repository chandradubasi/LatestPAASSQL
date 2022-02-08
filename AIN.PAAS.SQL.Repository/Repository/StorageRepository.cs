using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Repository.Repository
{
    public class StorageRepository : IStorageRepository
    {
        private readonly AINDatabaseContext _aINDatabaseContext;
        private readonly RegionConfigs _regionConfigs;
        public StorageRepository(AINDatabaseContext aINDatabaseContext, IOptions<RegionConfigs> regionConfigs)
        {
            _aINDatabaseContext = aINDatabaseContext;
            _regionConfigs = regionConfigs.Value;
        }

        public async Task<List<StoragesResponse>> GetStorages()
        {
            List<StoragesResponse> storagesList = new List<StoragesResponse>();
            try
            {
                var storages = await _aINDatabaseContext.Storage.Include(storage => storage.InventoryItem).ToListAsync();

                if (storages == null)
                {
                    return null;
                }
                else
                {
                    foreach (var storage in storages)
                    {
                        StoragesResponse storagesResponse = new StoragesResponse()
                        {
                            StorageId = storage.Id.ToString(),
                            Name = storage.Name,
                            Inventory = storage.InventoryItem.Select(I => _regionConfigs.APIDomain + "/api/inventory/" + I.Id).ToArray(),
                        };

                        storagesList.Add(storagesResponse);
                    }

                    return storagesList;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<StoragesResponse> GetStoragesById(string storageId)
        {
            try
            {
                var storages = await _aINDatabaseContext.Storage.Where(s => s.Id == Guid.Parse(storageId)).Include(s => s.InventoryItem).FirstOrDefaultAsync();
                if (storages == null)
                {
                    return null;
                }
                else
                {
                    StoragesResponse storagesResponse = new StoragesResponse()
                    {
                        StorageId = storages.Id.ToString(),
                        Name = storages.Name,
                        Inventory = storages.InventoryItem.Select(I => _regionConfigs.APIDomain + "/api/inventory/" + I.Id).ToArray(),
                    };
                    return storagesResponse;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
