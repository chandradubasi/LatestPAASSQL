using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class StoragesResponse
    {
        public string StorageId { get; set; }
        public string Name { get; set; }
        public string[] Inventory { get; set; }
    }
}
