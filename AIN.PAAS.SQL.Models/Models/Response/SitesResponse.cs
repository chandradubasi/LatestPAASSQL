﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    public class SitesResponse
    {
        public string SiteId { get; set; }
        public string Name { get; set; }
        public string[] Labs { get; set; }
    }
}
