using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GenericPOSRestService.Common.ServiceCallClasses;

namespace GenericPOSRestService.Common
{
    public static class CommonUtils
    {
        private static Dictionary<Errors, ErrorMessage> errorMessages = new Dictionary<Errors, ErrorMessage> {
            { Errors.POSError, new ErrorMessage { ShortMessage = "POS unknown error",  LongMessage = "POS error: {0}" } },
            { Errors.OrderNotFound, new ErrorMessage { LongMessage = "The order with OrderID = {0} does not exist" } },
            { Errors.ItemNotFound, new ErrorMessage { LongMessage = "The item with ID = {0} does not exist" } },
            { Errors.ItemListNotSpecified, new ErrorMessage { LongMessage = "The item list was not specified" } },
            { Errors.OrderIDNotSpecified, new ErrorMessage { LongMessage = "The OrderID value was not specified" } },
            { Errors.TenderNotFound, new ErrorMessage { LongMessage = "The tender with MediaID = {0} does not exist" } },
            { Errors.TenderItemListNotSpecified, new ErrorMessage { LongMessage = "The tender item list was not specified" } },
            { Errors.TerminalNotReady, new ErrorMessage { ShortMessage = "Terminal not ready", LongMessage = "The terminal used for kiosk {0} is not configured or is not ready for receive orders" } },
            { Errors.KioskNotSpecified, new ErrorMessage { ShortMessage = "Wrong parameters", LongMessage = "The Kiosk parameter was not specified" } },
            { Errors.RefIntNotSpecified, new ErrorMessage { LongMessage = "The RefInt parameter was not specified" } },
            { Errors.ErrorDeserializeRequest, new ErrorMessage { LongMessage = "Error deserialize request: {0}" } },
            { Errors.ThereIsAnOrderWithKioskAndRefInt, new ErrorMessage { LongMessage = "There is already a order with Kisok = {0} and RefInt = {1}" } },
            { Errors.CannotDoActionOnOrderBecauseIsNotOpen, new ErrorMessage { LongMessage = "Cannot {0} the order with OrderID = {1} because is not an open order" } },
            { Errors.MethodNotSpecified, new ErrorMessage { ShortMessage = "Wrong parameters", LongMessage = "The Method parameter was not specified" } },
            { Errors.CultureNameNotSpecified, new ErrorMessage { LongMessage = "The CultureName parameter was not specified" } },
            { Errors.ErrorCallingPOSMethod, new ErrorMessage { LongMessage = "Error calling POS method {0}" } },
            { Errors.JsonIncorrectFormat, new ErrorMessage { ShortMessage = "Json incorrect format", LongMessage = "Json incorrect format" } },
            { Errors.OrderMissing, new ErrorMessage { ShortMessage = "Order missing", LongMessage = "Order was not specified" } }
        };

        public static Dictionary<Errors, ErrorMessage> ErrorMessages
        {
            get
            {
                return errorMessages;
            }
        }

        /// <summary>Performs a safe threading action</summary>
        /// <param name="control">The invoked control</param>
        /// <param name="action">The action performed</param>
        public static void SafeStartThread(Control control, System.Action action)
        {
            if (control.InvokeRequired)
            {
                IAsyncResult result = control.BeginInvoke(new MethodInvoker(action));
                control.EndInvoke(result);
                result.AsyncWaitHandle.Close();
            }
            else
            {
                action();
            }
        }

        public static string GetMessage(this Nancy.HttpStatusCode statusCode)
        {
            switch (statusCode)
            { 
                case Nancy.HttpStatusCode.OK:
                    return "OK";

                case Nancy.HttpStatusCode.Unauthorized:
                    return "Unauthorized";

                case Nancy.HttpStatusCode.NotFound:
                    return "Not found";

                case Nancy.HttpStatusCode.Forbidden:
                    return "Forbidden";

                default:
                    return "Technical error";
            }
        }

        /// <summary>Extract the list of substrings delimited by two tags</summary>
        /// <param name="text">The text to parse</param>
        /// <param name="startTag">Start tag</param>
        /// <param name="endTag">End tag</param>
        private static List<string> ExtractFromString(
            string text,
            string startTag,
            string endTag)
        {
            List<string> matched = new List<string>();

            int indexStart = 0, indexEnd = 0;

            bool exit = false;

            while (!exit)
            {
                indexStart = text.IndexOf(startTag);
                indexEnd = text.IndexOf(endTag);

                if ((indexStart != -1) && (indexEnd != -1) && (indexEnd > indexStart))
                {
                    matched.Add(text.Substring(
                        indexStart + startTag.Length,
                        indexEnd - indexStart - startTag.Length));

                    text = text.Substring(indexEnd + endTag.Length);
                }
                else
                {
                    exit = true;
                }
            }

            return matched;
        }

        /// <summary>Returns the error message</summary>
        /// <param name="error">The errorto parse</param>
        /// <param name="longMessage">Specifies if long message is returned (true) or short message (false)</param>
        /// <param name="parameters">The parameters used to format the message (if needed)</param>
        public static string GetMessage(this Errors error, bool longMessage, params object[] parameters)
        {
            string msg = null;
            
            if (ErrorMessages.ContainsKey(error))
            {
                ErrorMessage errMessage = ErrorMessages[error];

                if (longMessage)
                {
                    msg = errMessage.LongMessage;

                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = errMessage.ShortMessage;
                    }
                }
                else
                {
                    msg = errMessage.ShortMessage;

                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        msg = errMessage.LongMessage;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(msg))
            {
                return "POS Unknown error";
            }
            else
            { 
                List<string> formatParams = ExtractFromString(msg, "{", "}");

                if (formatParams.Any())
                {
                    for (int i = 0; i < formatParams.Count; i++)
                    {
                        int index = formatParams[i].IndexOf(':');

                        if (index > 0)
                        {
                            formatParams[i] = formatParams[i].Substring(0, index);
                        }
                    }

                    int cnt = formatParams.Distinct().Count();

                    object[] _parameters = new object[cnt];

                    if ((parameters != null) && (parameters.Length >= cnt))
                    {
                        Array.Copy(parameters, _parameters, cnt);
                    }
                    else
                    {
                        if (parameters != null)
                        {
                            Array.Copy(parameters, _parameters, parameters.Length);

                            for (int i = parameters.Length; i < cnt; i++)
                            {
                                _parameters[i] = "";
                            }
                        }
                        else
                        {
                            for (int i = 0; i < cnt; i++)
                            {
                                _parameters[i] = "";
                            }
                        }
                    }

                    msg = string.Format(msg, _parameters);
                }
            }

            return msg;
        }
    }
}
