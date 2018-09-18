using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class Order
    {
        public string OrderID { get; set; }
        public string Kiosk { get; set; }
        public string RefInt { get; set; }
        public long OrderPOSNumber { get; set; }
        public Closed Closed { get; set; }
        public string POSStatus { get; set; }
        public string Receipt { get; set; }
        public string Elog { get; set; }

        private string reason = "Success";

        public string Reason
        {
            get
            {
                return this.HasErrors ? reason : "Success";
            }
            set
            {
                reason = value;
            }
        }

        private Totals totals = new Totals();

        public Totals Totals
        {
            get
            {
                if (totals == null)
                {
                    totals = new Totals();
                }

                return totals;
            }
            set
            {
                totals = value;
            }
        }

        public bool HasErrors
        {
            get
            {
                int intValue = 0;
                bool hasErrors = false;

                if (int.TryParse(OrderID, out intValue) && (intValue < 0))
                {
                    hasErrors = true;
                }

                return hasErrors;
            }
        }
    }
}
