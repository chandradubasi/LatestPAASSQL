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
    public class LabsService : ILabsService
    {

        private readonly ILabsRepository  _labsRepository;
        public LabsService(ILabsRepository labsRepository)
        {
            _labsRepository = labsRepository;
        }
        public  async Task<LabsResponse> GetLab(string labID)
        {
            return await _labsRepository.GetLab(labID);

        }

        public async Task<List<LabsResponse>> GetLabs()
        {
            return await _labsRepository.GetLabs();
        }
    }
}
