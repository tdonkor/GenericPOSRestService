using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class TaxItem
    {
        public string TaxID { get; set; }
        public long TaxAmount { get; set; }
        public string TaxType { get; set; }
        public long TaxRate { get; set; }
    }
}
