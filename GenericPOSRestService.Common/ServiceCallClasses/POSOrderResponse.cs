using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class POSOrderResponse
    {
        public string OrderID { get; set; }
        public long OrderPOSNumber { get; set; }
        public string Kiosk { get; set; }
        public string RefInt { get; set; }
        public string Reason { get; set; }
        public string Receipt { get; set; }
        
    }
}
