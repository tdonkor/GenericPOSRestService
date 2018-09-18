using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class StatusResponse
    {
        private bool status = true;

        public string Status
        {
            get
            {
                return status ? "OK" : "NOK";
            }
            set
            {
                status = value.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public string BusinessDay { get; set; }

        private string customerReason = "";

        public string CustomerReason
        {
            get
            {
                return status
                    ? ""
                    : string.IsNullOrWhiteSpace(customerReason)
                        ? "Unknown error"
                        : customerReason;
            }
            set
            {
                customerReason = value;
            }
        }

        private string technicalReason = "";

        public string TechnicalReason
        {
            get
            {
                return status
                    ? ""
                    : string.IsNullOrWhiteSpace(technicalReason)
                        ? "Unknown error"
                        : technicalReason;
            }
            set
            {
                technicalReason = value;
            }
        }

        public string DataVersion { get; set; }
    }
}
