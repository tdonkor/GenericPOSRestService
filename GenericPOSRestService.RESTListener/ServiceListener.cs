using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Hosting.Self;
using GenericPOSRestService.Common;

namespace GenericPOSRestService.RESTListener
{
    /// <summary>The REST service controller class</summary>
    public sealed partial class ServiceListener : IDisposable
    {
        #region Instance

        private static volatile ServiceListener instance;
        private static object syncRoot = new Object();

        public static ServiceListener Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ServiceListener();
                        }
                    }
                }

                return instance;
            }
        }
        #endregion

        public event EventHandler<WriteToLogEventArgs> WriteToLog;

        public void OnWriteToLog(WriteToLogEventArgs e)
        {
            EventHandler<WriteToLogEventArgs> handler = WriteToLog;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        private NancyHost ServiceHost { get; set; }

        private string logName = "RESTListener";

        public string LogName
        {
            get
            {
                return logName;
            }
            private set
            {
                logName = value;
            }
        }

        public bool StartRESTServiceListener()
        {
            bool result = true;

            if (ServiceHost == null)
            {
                result = false;

                try
                {
                    HostConfiguration hostConfiguration = new HostConfiguration();
                    hostConfiguration.UrlReservations.CreateAutomatically = true;

                    ServiceHost = new NancyHost(hostConfiguration, ListenerConfig.Instance.POSRESTRootUrl);
                    ServiceHost.Start();
                    Log.Debug(LogName, string.Format("REST service {0} started", ListenerConfig.Instance.POSRESTRootUrl));
                    result = true;
                }
                catch (Exception ex)
                {
                    ServiceHost = null;
                    Log.Error(LogName, string.Format("Error starting REST service {0}: {1}", ListenerConfig.Instance.POSRESTRootUrl, ex.ToString()));
                }
            }

            return result;
        }

        public void StopRESTServiceListener()
        {
            if (ServiceHost != null)
            {
                try
                {
                    ServiceHost.Stop();
                    Log.Debug(LogName, string.Format("REST service {0} stopped", ListenerConfig.Instance.POSRESTRootUrl));
                }
                catch (Exception ex)
                {
                    Log.Error(LogName, string.Format("Error stopping REST service {0}: {1}", ListenerConfig.Instance.POSRESTRootUrl, ex.ToString()));
                }

                ServiceHost = null;
            }
        }

        ~ServiceListener()
        {
            Dispose();
        }

        public void Dispose()
        {
            StopRESTServiceListener();
        }
    }
}
