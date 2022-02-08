using AIN.PAAS.SQL.Models.Models.Response;
using AIN.PAAS.SQL.Repository.IRepository;
using AIN.PAAS.SQL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Services.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository  _hospitalRepository;
        public HospitalService(IHospitalRepository hospitalRepository)
        {
            _hospitalRepository = hospitalRepository;
        }

        public async Task<List<HospitalsResponse>> GetHospital()
        {
            return await _hospitalRepository.GetHospital();
        }
    }
}
