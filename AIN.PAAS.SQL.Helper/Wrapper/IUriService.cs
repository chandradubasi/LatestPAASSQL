using AIN.PAAS.SQL.Models.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Helper.Wrapper
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
