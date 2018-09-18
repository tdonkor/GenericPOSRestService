using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class TenderItem
    {
        public string MediaID { get; set; }
        public long Amount { get; set; }
        public string Reference { get; set; }
    }
}
