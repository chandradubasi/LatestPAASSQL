using AIN.PAAS.SQL.Helper.Wrapper;
using AIN.PAAS.SQL.Models.Models;
using AIN.PAAS.SQL.Models.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Helper
{    

    public static class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            if (totalRecords == 0)
            {
                respose.CurrentPage = null;
                respose.NextPage = null;
                respose.PreviousPage = null;
            }
            else
            {
                respose.CurrentPage = uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber, validFilter.PageSize), route);
                respose.NextPage = uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route);
                respose.PreviousPage = uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route);

                //respose.NextPage =
                //    validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                //    ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
                //    : null;

                //respose.PreviousPage =
                //    validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                //    ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
                //    : null;
                //respose.FirstPage = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
                // respose.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
            }
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;
            return respose;
        }
        
    }
}
