using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class LabsResponse
    {
        public string LabId { get; set; }
        public string Name { get; set; }
        public string[] Locations { get; set; }
    }
}
