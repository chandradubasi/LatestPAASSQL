using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 50;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize;//pageSize > 10 ? 10 : pageSize;
        }
    }
}
