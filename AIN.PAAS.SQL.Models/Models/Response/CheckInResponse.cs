using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class CheckInResponse
    {
        public string Status { get; set; }
        public string[] SGTIN { get; set; }
        public string Product { get; set; }
        public string Lot { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Storage { get; set; }
    }
}
