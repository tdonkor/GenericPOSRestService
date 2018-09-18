using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class OrderCreatePOSResponse : IPOSResponse
    {
        private OrderCreateResponse orderCreateResponse = new OrderCreateResponse();

        public OrderCreateResponse OrderCreateResponse
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.Created)
                {
                    if (orderCreateResponse == null)
                    {
                        orderCreateResponse = new OrderCreateResponse();
                    }

                    return orderCreateResponse;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                orderCreateResponse = value;
            }
        }

        public string ResponseContent
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.Created)
                {
                    return JObject.FromObject(OrderCreateResponse).ToString();
                }
                else
                {
                    return HttpStatusCode.GetMessage();
                }
            }
        }

        private Nancy.HttpStatusCode httpStatusCode = Nancy.HttpStatusCode.Created;

        public Nancy.HttpStatusCode HttpStatusCode
        {
            get
            {
                return new Nancy.HttpStatusCode[]
                {
                    Nancy.HttpStatusCode.Created,
                    Nancy.HttpStatusCode.Forbidden,
                    Nancy.HttpStatusCode.Unauthorized,
                    Nancy.HttpStatusCode.NotFound
                }.Contains(httpStatusCode)
                    ? httpStatusCode
                    : Nancy.HttpStatusCode.InternalServerError;
            }
            set
            {
                httpStatusCode = value;
            }
        }

        public void SetPOSError(Errors posError, params object[] parameters)
        {
            if ((OrderCreateResponse != null) || (orderCreateResponse != null))
            {
                orderCreateResponse.Order.OrderID = ((int)posError).ToString();
                orderCreateResponse.Order.Reason = posError.GetMessage(true, parameters);
            }
        }
    }
}
