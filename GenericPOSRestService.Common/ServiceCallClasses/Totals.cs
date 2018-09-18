using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class Totals
    {
        public long AmountPaid { get; set; }
        public long AmountDue { get; set; }

        private TaxItems taxItems = new TaxItems();

        public TaxItems TaxItems
        {
            get
            {
                if (taxItems == null)
                {
                    taxItems = new TaxItems();
                }

                return taxItems;
            }
            set
            {
                taxItems = value;
            }
        }
    }
}
