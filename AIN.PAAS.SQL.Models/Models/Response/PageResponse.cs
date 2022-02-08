using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIN.PAAS.SQL.Models.Models.Response
{
    //class PageResponse
    //{

    //}

    public class PageResponse<T>
    {
        public PageResponse()
        {
        }
        public PageResponse(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
        public T Data { get; set; }
        public bool Succeeded { get; set; }

        public string[] Errors { get; set; }
        public string Message { get; set; }
    }
}
