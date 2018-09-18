using GenericPOSRestService.RESTListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPOSRestService.HostService
{
    public class GenericRestService
    {
        public GenericRestService()
        {
        }

        public void Start()
        {
            ServiceListener.Instance.StartRESTServiceListener();
        }

        public void Stop()
        {
            ServiceListener.Instance.StopRESTServiceListener();
        }
    }
}
