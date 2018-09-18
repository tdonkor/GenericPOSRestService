using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using System.Diagnostics;
using System.Threading.Tasks;
using GenericPOSRestService.Common;
using GenericPOSRestService.Common.ServiceCallClasses;
using Newtonsoft.Json.Linq;
using Nancy.Responses;
using Newtonsoft.Json;

namespace GenericPOSRestService.RESTListener
{
    /// <summary>The REST listener module</summary>
    public class RESTNancyModule : NancyModule
    {
        /// <summary>Formatted string for writing in the log on a service request</summary>
        private const string LogRequestString = "REST service call \"{0}\" => request: {1}";

        /// <summary>Formatted string for writing in the log on a service response</summary>
        private const string LogResponseString = "REST service call \"{0}\" =>\r\trequest: {1}\r\tresponse: {2}\r\tCalculationTimeInMilliseconds: {3}";

        private const string LogResponseSkipRequestString = "REST service call \"{0}\" => response: {2}\r\tCalculationTimeInMilliseconds: {3}";

        public string LogName
        {
            get
            {
                return ServiceListener.Instance.LogName;
            }
        }

        public RESTNancyModule()
            : base(ListenerConfig.Instance.POSRESTModuleBase)
        {
            Get["/status/{kiosk?}"] = parameters =>
            {
                // try to get the kiosk parameter
                string kiosk = null;

                try
                {
                    string kioskStr = parameters.kiosk;

                    if (!string.IsNullOrWhiteSpace(kioskStr))
                    {
                        kiosk = kioskStr;
                    }
                }
                catch
                {
                }

                // defines the function for calling GetStatus method
                Func<string, IPOSResponse> func = (bodyStr) =>
                {
                    StatusPOSResponse statusPOSResponse = new StatusPOSResponse();

                    if (string.IsNullOrWhiteSpace(kiosk))
                    {
                        // the kiosk parameter was not specified
                        statusPOSResponse.SetPOSError(Errors.KioskNotSpecified);
                    }
                    else
                    {
                        try
                        {
                            // call the POS and get the status for the specified kiosk
                            statusPOSResponse = GetStatus(kiosk);
                        }
                        catch (Exception ex)
                        {
                            statusPOSResponse = new StatusPOSResponse();
                            statusPOSResponse.SetPOSError(Errors.POSError, ex.Message);
                        }
                    }

                    return statusPOSResponse;
                };

                // call GetStatus function
                IPOSResponse response = ExecuteRESTCall(func);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return new TextResponse(response.HttpStatusCode, response.ResponseContent);
                }
                else
                {
                    return response.HttpStatusCode;
                }
            };

            Post["/order"] = parameters =>
            {
                // defines the function for calling OrderTransaction method
                Func<string, IPOSResponse> func = (bodyStr) =>
                {
                    OrderCreatePOSResponse posResponse = new OrderCreatePOSResponse();
                    Order order = posResponse.OrderCreateResponse.Order;
                    OrderCreateRequest request = null;

                    try
                    {
                        // deserialize request
                        request = JsonConvert.DeserializeObject<OrderCreateRequest>(bodyStr);
                    }
                    catch(Exception ex)
                    {
                        posResponse.SetPOSError(Errors.ErrorDeserializeRequest, ex.Message);
                    }

                    if (!order.HasErrors)
                    {
                        // no deserialize errors => check some elements
                        if (request.DOTOrder == null)
                        {
                            posResponse.SetPOSError(Errors.OrderMissing);
                        }
                        else if (string.IsNullOrWhiteSpace(request.DOTOrder.Kiosk))
                        {
                            posResponse.SetPOSError(Errors.KioskNotSpecified);
                        }
                        else if (string.IsNullOrWhiteSpace(request.DOTOrder.RefInt))
                        {
                            posResponse.SetPOSError(Errors.RefIntNotSpecified);
                        }
                        else if (request.DOTOrder.IsNewOrder && !request.DOTOrder.Items.Any())
                        {
                            posResponse.SetPOSError(Errors.ItemListNotSpecified);
                        }
                        else if (request.DOTOrder.IsTenderOrder
                            && ((request.DOTOrder.Tender == null)
                                || (request.DOTOrder.Tender.TenderItems == null)
                                || !request.DOTOrder.Tender.TenderItems.Any()))
                        {
                            posResponse.SetPOSError(Errors.TenderItemListNotSpecified);
                        }
                        else if (request.DOTOrder.IsExistingOrder && string.IsNullOrWhiteSpace(request.DOTOrder.OrderID))
                        {
                            posResponse.SetPOSError(Errors.OrderIDNotSpecified);
                        }
                    }

                    if (!order.HasErrors)
                    {
                        try
                        {
                            posResponse = OrderTransaction(request);
                        }
                        catch (Exception ex)
                        {
                            posResponse = new OrderCreatePOSResponse();
                            posResponse.SetPOSError(Errors.POSError, ex.Message);
                        }
                    }

                    return posResponse;
                };

                // call OrderTransaction method
                IPOSResponse response = ExecuteRESTCall(func);

                if (response.HttpStatusCode == HttpStatusCode.Created)
                {
                    return new TextResponse(response.HttpStatusCode, response.ResponseContent);
                }
                else
                {
                    return response.HttpStatusCode;
                }
            };

            Get["/testdiag/{culturename?}"] = parameters =>
            {
                // try to get the culture name
                string culturename = null;

                try
                {
                    culturename = parameters.culturename;
                }
                catch
                {
                }

                // defines the function for calling TestDiag method
                Func<string, IPOSResponse> func = (bodyStr) =>
                {
                    TestDiagPOSResponse posResponse = new TestDiagPOSResponse();

                    if (string.IsNullOrWhiteSpace(culturename))
                    {
                        posResponse.SetPOSError(Errors.CultureNameNotSpecified);
                    }
                    else
                    {
                        try
                        {
                            posResponse = TestDiag(culturename);
                        }
                        catch (Exception ex)
                        {
                            posResponse = new TestDiagPOSResponse();
                            posResponse.SetPOSError(Errors.POSError, ex.Message);
                        }
                    }

                    return posResponse;
                };

                // call TestDiag method
                IPOSResponse response = ExecuteRESTCall(func);

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return new TextResponse(response.HttpStatusCode, response.ResponseContent);
                }
                else
                {
                    return response.HttpStatusCode;
                }
            };
        }

        /// <summary>Writes the message to the log file</summary>
        /// <param name="message">The message</param>
        /// <param name="methodName">The method</param>
        /// <param name="requestContent">The request content</param>
        /// <param name="level">The log level</param>
        private void WriteLog(
            string message,
            string methodName,
            string requestContent,
            LogLevel level)
        {
            // write the message
            switch (level)
            { 
                case LogLevel.Debug:
                    Log.Debug(LogName, message);
                    break;

                case LogLevel.Error:
                    Log.Error(LogName, message);
                    break;

                case LogLevel.Warnings:
                    Log.Warnings(LogName, message);
                    break;

                case LogLevel.Info:
                    Log.Info(LogName, message);
                    break;

                case LogLevel.Windows:
                    Log.WindowsError(LogName, message);
                    break;

                default:
                    Log.Sys(LogName, message);
                    break;
            }

            // raise OnWriteToLog event
            ServiceListener.Instance.OnWriteToLog(new WriteToLogEventArgs
            {
                MethodName = methodName,
                RequestContent = requestContent,
                Message = message
            });
        }

        /// <summary>Generic method for execute the REST call</summary>
        /// <param name="func">The cal REST function </param>
        private IPOSResponse ExecuteRESTCall(System.Func<string, IPOSResponse> func)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            string bodyStr = Request.Body.ReadAsString();
            string restUrl = Request.Url.ToString();
            string requestIP = Request.UserHostAddress;

            if (requestIP == "::1")
            {
                requestIP = "localhost";
            }

            int lastIndex = Request.Url.Path.LastIndexOf('/');

            string methodName = lastIndex >= 0 ? Request.Url.Path.Substring(lastIndex + 1) : Request.Url.Path;

            string logRequestString = AddPrefixMessage(
                GetLogRequestString(restUrl, bodyStr),
                requestIP);

            // log request
            WriteLog(logRequestString, Request.Method, bodyStr, LogLevel.Debug);

            // call the function
            IPOSResponse response = func(bodyStr);

            sw.Stop();

            string logResponseString = AddPrefixMessage(
                GetLogResponseString(restUrl, bodyStr, response.ResponseContent, sw.ElapsedMilliseconds),
                requestIP);

            // log response
            WriteLog(logResponseString, Request.Method, bodyStr, LogLevel.Debug);

            return response;
        }

        private string AddPrefixMessage(string message, string requestIP)
        {
            string prefixMsg = "";

            if (!string.IsNullOrWhiteSpace(requestIP))
            {
                prefixMsg = string.Format("Request IP: {0}", requestIP);
            }

            return prefixMsg + (string.IsNullOrWhiteSpace(prefixMsg) ? "" : ", ") + message;
        }

        /// <summary>Returns the request message for writing in the log file</summary>
        /// <param name="url">The url</param>
        /// <param name="request">The request</param>
        private string GetLogRequestString(string url, string request)
        {
            return string.Format(LogRequestString, url, request);
        }

        /// <summary>Returns the response message for writing in the log file</summary>
        /// <param name="url">The url</param>
        /// <param name="response">The response</param>
        private string GetLogResponseString(string url, string request, string response, long calculationTimeInMilliseconds, bool skipRequest = false)
        {
            return string.Format(skipRequest ? LogResponseSkipRequestString : LogResponseString, url, request, response, calculationTimeInMilliseconds);
        }

        /// <summary>Call the POS for GetStatus method</summary>
        /// <param name="kiosk">The kiosk id</param>
        public StatusPOSResponse GetStatus(string kiosk)
        {
            StatusPOSResponse response = new StatusPOSResponse();

            // TODO: call (calls) to POS

            return response;
        }

        /// <summary>Call the POS for Order method</summary>
        /// <param name="request">The request</param>
        public OrderCreatePOSResponse OrderTransaction(OrderCreateRequest request)
        {
            OrderCreatePOSResponse response = new OrderCreatePOSResponse();

            // TODO: call (calls) to POS

            return response;
        }

        /// <summary>Call the POS for TestDiag method</summary>
        /// <param name="cultureName">The culture name</param>
        public TestDiagPOSResponse TestDiag(string cultureName)
        {
            TestDiagPOSResponse response = new TestDiagPOSResponse();

            // TODO: call (calls) to POS

            return response;
        }
    }
}
