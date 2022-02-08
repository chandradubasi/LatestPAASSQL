using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Request
{
   public class FilterFieldsInput
    {

        public Guid Id { get; set; }
        public string Sgtin { get; set; }
        public Guid? StorageId { get; set; }
        public DateTime? ExpiryDate { get; set; }       
        public Guid? ProductId { get; set; }       
        public string Status { get; set; }

    }
}
