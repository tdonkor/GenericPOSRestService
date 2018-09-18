using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public class TestDiagPOSResponse : IPOSResponse
    {
        private TestDiagResponse testDiagResponse = new TestDiagResponse();

        public TestDiagResponse TestDiagResponse
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.OK)
                {
                    if (testDiagResponse == null)
                    {
                        testDiagResponse = new TestDiagResponse();
                    }

                    return testDiagResponse;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                testDiagResponse = value;
            }
        }

        public string ResponseContent
        {
            get
            {
                if (HttpStatusCode == Nancy.HttpStatusCode.OK)
                {
                    return JObject.FromObject(TestDiagResponse).ToString();
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
            if ((TestDiagResponse != null) || (testDiagResponse != null))
            {
                testDiagResponse.ReturnCode = (int)posError;
                testDiagResponse.ReturnMessage = posError.GetMessage(false, parameters);
            }
        }
    }
}
