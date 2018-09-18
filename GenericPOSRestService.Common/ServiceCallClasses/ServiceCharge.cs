using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class ServiceCharge
    {
        public long Total { get; set; }
        public List<ServiceItem> ServiceItems { get; set; }
    }
}
