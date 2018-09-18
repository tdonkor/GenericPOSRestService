using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.IO;
using System.IO;

namespace GenericPOSRestService.RESTListener
{
    public static class ServiceListenerUtils
    {
        /// <summary>Return string for RequestStream</summary>
        /// <param name="requestStream">Request stream to read</param>
        public static string ReadAsString(this RequestStream requestStream)
        {
            using (var reader = new StreamReader(requestStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
