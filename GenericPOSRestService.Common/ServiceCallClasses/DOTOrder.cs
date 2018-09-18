using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class DOTOrder
    {
        public Location Location { get; set; }
        public Status Status { get; set; }
        public FunctionNumber FunctionNumber { get; set; }
        public string StatusDetail { get; set; }
        public string Kiosk { get; set; }
        public string RefInt { get; set; }
        public string OrderID { get; set; }
        public string Day { get; set; }
        public string OrderTime { get; set; }
        public string EventTime { get; set; }
        public string Elog { get; set; }

        private List<Item> items = new List<Item>();

        public List<Item> Items
        {
            get
            {
                if (items == null)
                {
                    items = new List<Item>();
                }

                return items;
            }
            set
            {
                items = value;
            }
        }

        public Discounts Discounts { get; set; }
        public ServiceCharge ServiceCharge { get; set; }

        private Tender tender = null;
        
        public Tender Tender
        {
            get
            {
                if ((tender == null) && IsTenderOrder)
                {
                    tender = new Tender();
                }

                return tender;
            }
            set
            {
                tender = value;
            }
        }

        public bool IsNewOrder
        {
            get
            {
                return (new FunctionNumber[] {
                        FunctionNumber.EXT_COMPLETE_ORDER,
                        FunctionNumber.EXT_OPEN_ORDER
                    }).Contains(FunctionNumber);
            }
        }

        public bool IsTenderOrder
        {
            get
            {
                return (new FunctionNumber[] {
                        FunctionNumber.EXT_COMPLETE_ORDER,
                        FunctionNumber.EXT_TENDER_ORDER
                    }).Contains(FunctionNumber);
            }
        }

        public bool IsExistingOrder
        {
            get
            {
                return (new FunctionNumber[] {
                        FunctionNumber.EXT_TENDER_ORDER,
                        FunctionNumber.EXT_VOID_ORDER
                    }).Contains(FunctionNumber);
            }
        }

        public long OrderAmount
        {
            get
            {
                long result = Items.Any() ? Items.Sum(r => r.ItemPrice) : 0;

                if (ServiceCharge != null)
                {
                    result += ServiceCharge.Total;
                }

                if (Discounts != null)
                {
                    result -= Discounts.Total;
                }

                return result;
            }
        }

        public long? PaidAmount
        {
            get
            { 
                return IsTenderOrder ? (long?)Tender.Total : null;
            }
        }
    }
}
