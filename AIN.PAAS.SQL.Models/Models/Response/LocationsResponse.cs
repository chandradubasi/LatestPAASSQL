using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class LocationsResponse
    {
        public string LocationId { get; set; }
        public string Name { get; set; }
        public string[] Storages { get; set; }
    }
}
