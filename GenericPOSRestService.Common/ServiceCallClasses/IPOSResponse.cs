using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.Common.ServiceCallClasses
{
    public interface IPOSResponse
    {
        Nancy.HttpStatusCode HttpStatusCode { get; set; }
        string ResponseContent { get; }
        void SetPOSError(Errors posError, params object[] parameters);
    }
}
