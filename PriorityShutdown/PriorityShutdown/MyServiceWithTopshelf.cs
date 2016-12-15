using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PriorityShutdown;
using Topshelf;

namespace WindowsServiceWithTopshelf
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            var serviceName = "PriorityShutdownWithTopshelf";
            HostFactory.Run(configure =>
            {
                configure.Service<MyService>(service =>
                {
                    service.ConstructUsing(s => new MyService(serviceName));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                    service.WhenShutdown(s => s.Shutdown());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.EnableShutdown();
                configure.DependsOnEventLog();
                configure.SetServiceName(serviceName);
                configure.SetDisplayName(serviceName);
                configure.BeforeInstall((settings) =>
                {
                    if (!System.Diagnostics.EventLog.SourceExists(settings.ServiceName))
                        System.Diagnostics.EventLog.CreateEventSource(settings.ServiceName, "Application");
                });
                configure.SetDescription("My .Net windows service with Topshelf");
            });
        }
    }
}
