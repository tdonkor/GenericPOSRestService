using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class Discounts
    {
        public long Total { get; set; }
        public List<DiscountItem> DiscountItem { get; set; }
    }
}
