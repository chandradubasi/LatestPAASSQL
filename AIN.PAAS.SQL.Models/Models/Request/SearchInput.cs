using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Request
{
   public class SearchInput
    {
        public enum SortBy
        {
            Asc,
            Desc
        }

       // public Guid Id { get; set; }
        public string Sgtin { get; set; }
        public Guid? StorageId { get; set; }

       // public DateTime? ExpiryDate { get; set; }        
        public Guid? ProductId { get; set; }

        //public SortBy? SortByExpiryDate { get; set; } = SortBy.Asc;
        public string SortByExpiryDate { get; set; }

        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 

    }
    
}
