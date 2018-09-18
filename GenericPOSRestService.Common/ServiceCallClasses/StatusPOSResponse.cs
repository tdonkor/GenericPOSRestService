using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using GenericPOSRestService.Common;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class StatusPOSResponse : IPOSResponse
    {
        private StatusResponse statusResponse = new StatusResponse();

        public StatusResponse StatusResponse
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.OK)
                {
                    if (statusResponse == null)
                    {
                        statusResponse = new StatusResponse();
                    }

                    return statusResponse;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                statusResponse = value;
            }
        }

        public string ResponseContent
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.OK)
                {
                    return JObject.FromObject(StatusResponse).ToString();
                }
                else
                {
                    return HttpStatusCode.GetMessage();
                }
            }
        }

        private Nancy.HttpStatusCode httpStatusCode = Nancy.HttpStatusCode.OK;

        public Nancy.HttpStatusCode HttpStatusCode
        {
            get
            {
                return new Nancy.HttpStatusCode[]
                {
                    Nancy.HttpStatusCode.OK,
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
            if ((StatusResponse != null) || (statusResponse != null))
            {
                statusResponse.Status = "NOK";
                statusResponse.CustomerReason = posError.GetMessage(false, parameters);
                statusResponse.TechnicalReason = posError.GetMessage(true, parameters);
            }
        }
    }
}
