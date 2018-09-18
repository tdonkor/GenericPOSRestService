using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class Item
    {
        public string ID { get; set; }

        private long qty = 1;

        public long Qty
        {
            get
            {
                if (qty < 1)
                {
                    qty = 1;
                }

                return qty;
            }
            set
            {
                qty = value;
            }
        }

        public long? Unitprice { get; set; }

        public long? Price { get; set; }

        public long? PromoPrice { get; set; }

        private bool visible = true;

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        public string PromoRef { get; set; }

        public List<Remark> Remarks { get; set; }

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

        public bool IsMenu
        {
            get
            {
                return Items.Any();
            }
        }

        public long ItemPrice
        {
            get
            {
                return IsMenu
                    ? Items.Sum(r => r.ItemPrice)
                    : PromoPrice.HasValue ? PromoPrice.Value : Price ?? 0;
            }
        }
    }
}
