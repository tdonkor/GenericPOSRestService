using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class DiscountItem
    {
        public string DiscountID { get; set; }
        public long Amount { get; set; }
        public string Reference { get; set; }
        public string DiscountName { get; set; }
        public long OriginalAmount { get; set; }
    }
}
