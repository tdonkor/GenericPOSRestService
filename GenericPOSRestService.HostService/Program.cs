using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace GenericPOSRestService.HostService
{
    class Program
    {
        static void Main(string[] args)
        {
            TopshelfExitCode rc = HostFactory.Run(x =>
            {
                x.Service<GenericRestService>(s =>
                {
                    s.ConstructUsing(name => new GenericRestService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Generic POS Rest Service");
                x.SetDisplayName("Generic POS Rest Service");
                x.SetServiceName("GenericPOSRestService");
            });

            int exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
