using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class TaxItems
    {
        public long Amount { get; set; }
        public List<TaxItem> TaxItem { get; set; }
    }
}
