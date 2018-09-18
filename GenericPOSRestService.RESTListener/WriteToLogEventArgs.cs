using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericPOSRestService.RESTListener
{
    public class WriteToLogEventArgs : EventArgs
    {
        public string MethodName { get; set; }
        public string RequestContent { get; set; }
        public string Message { get; set; }
    }
}
