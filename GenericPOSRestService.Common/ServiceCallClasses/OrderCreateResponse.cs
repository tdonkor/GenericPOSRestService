using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class OrderCreateResponse
    {
        private Order order = new Order();

        public Order Order
        {
            get
            {
                if (order == null)
                {
                    order = new Order();
                }

                return order;
            }
            set
            {
                order = value;
            }
        }
    }
}
