using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class HospitalsResponse
    {
        public string HospitalId { get; set; }
        public string Name { get; set; }
        public string[] Sites { get; set; }
    }
}
