using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class Tender
    {
        public long Total
        {
            get
            {
                return TenderItems.Any() ? TenderItems.Sum(r => r.Amount) : 0;
            }
        }

        public int paid = 0;

        public int Paid
        {
            get
            {
                return paid == 0 ? paid : 1;
            }
            set
            {
                paid = value;
            }
        }

        private List<TenderItem> tenderItems = new List<TenderItem>();

        public List<TenderItem> TenderItems
        {
            get
            {
                if (tenderItems == null)
                {
                    tenderItems = new List<TenderItem>();
                }

                return tenderItems;
            }
            set
            {
                tenderItems = value;
            }
        }
    }
}
