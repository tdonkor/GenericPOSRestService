using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using GenericPOSRestService.Common;

namespace GenericPOSRestService.RESTListener
{
    /// <summary>Configuration class used by REST listener</summary>
    public sealed partial class ListenerConfig
    {
        #region Instance

        private static volatile ListenerConfig instance;
        private static object syncRoot = new Object();

        public static ListenerConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ListenerConfig();
                        }
                    }
                }

                return instance;
            }
        }
        #endregion

        private Uri posRESTRootUrl = new Uri("http://localhost:9730");

        public Uri POSRESTRootUrl
        {
            get
            {
                return posRESTRootUrl;
            }
            set
            {
                posRESTRootUrl = value;
            }
        }

        private string posRESTModuleBase = "/POS";

        public string POSRESTModuleBase
        {
            get
            {
                return string.IsNullOrWhiteSpace(posRESTModuleBase) ? posRESTModuleBase : "/" + posRESTModuleBase.Trim('/');
            }
            set
            {
                posRESTModuleBase = value;
            }
        }

        public string LogName
        {
            get
            {
                return ServiceListener.Instance.LogName;
            }
        }

        public Uri POSRESTUrl
        {
            get
            {
                return new Uri(posRESTRootUrl.ToString().TrimEnd('/') + "/" + POSRESTModuleBase.Trim('/'), UriKind.Absolute);
            }
        }

        private void LoadConfig()
        {
            string filePath = Properties.Settings.Default.GenericInjectorConfigFileName;

            filePath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(Application.StartupPath, filePath);

            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
            {
                try
                {
                    XElement xe = XElement.Load(filePath);

                    XElement xCrtElem = xe.Element("POSRESTRootUrl");

                    Uri uri;

                    if ((xCrtElem != null) && (Uri.TryCreate(xCrtElem.Value, UriKind.Absolute, out uri)))
                    {
                        POSRESTRootUrl = uri;
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(LogName, string.Format("Error reading file \"{0}\": {1}", filePath, ex.Message));
                }
            }
        }

        private ListenerConfig()
            : base()
        {
            LoadConfig();
        }
    }
}
