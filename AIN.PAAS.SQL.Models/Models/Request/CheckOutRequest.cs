using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Request
{
    public class CheckOutRequest
    {
        public string ProductId { get; set; }
        public string StorageId { get; set; }
        public string LotNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string[] SGTIN { get; set; }
    }
}
