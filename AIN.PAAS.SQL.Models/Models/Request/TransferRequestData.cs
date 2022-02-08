using System;
using System.Collections.Generic;
using System.Text;

namespace AIN.PAAS.SQL.Models.Models.Request
{
    public class TransferRequestData
    {
        public string SourceId { get; set; }
        public string DestinationId { get; set; }
        public string Requester { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Approver { get; set; }
        public string Status { get; set; }
        public string[] items { get; set; }
        public string Comments { get; set; }
    }
}
