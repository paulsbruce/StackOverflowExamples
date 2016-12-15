using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// illustrating the point for
// http://stackoverflow.com/questions/40918267/c-sharp-service-invokemethod-fails-during-windows-shutdown

namespace PriorityShutdown
{
    public class MyService
    {
        public string ServiceName { get; private set; }

        public MyService() { }
        public MyService(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        [DllImport("kernel32.dll")]
        static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);

        public void Start()
        {
            Log("Starting");

            SetProcessShutdownParameters(0x3FF, 0x00000001);

            SetDNS();
            Log("Started");
        }
        public void Stop()
        {
            Log("Stopping");
            ClearDNS();
            Log("Stopped");
        }
        public void Shutdown()
        {
            ClearDNS();
            Log("Shutdown");
        }

        private void Log(string s)
        {
            System.Diagnostics.EventLog.WriteEntry(this.ServiceName, Debug(s), System.Diagnostics.EventLogEntryType.Information);
        }
        private string Debug(string s)
        {
            var sOut = String.Format("{2} [{0}] {1}", DateTime.Now, s, this.ServiceName);
            Console.WriteLine(sOut);
            return sOut;
        }

        private void SetDNS()
        {
            _ConfigureDNS((objMO =>
            {
                ManagementBaseObject newDNS = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                newDNS["DNSServerSearchOrder"] = "192.168.1.24,192.168.1.2".Split(',');
                ManagementBaseObject setDNS = objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                Log("Successfully SetDNS");
            }));
        }
        private void ClearDNS()
        {
            _ConfigureDNS((objMO =>
            {
                ManagementBaseObject newDNS = objMO.GetMethodParameters("SetDNSServerSearchOrder");
                newDNS["DNSServerSearchOrder"] = null;
                ManagementBaseObject setDNS = objMO.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                Log("Successfully ClearDNS");
            }));
        }
        private void _ConfigureDNS(Action<ManagementObject> act)
        {
            using (var objMC = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (var objMOC = objMC.GetInstances())
                {
                    foreach (ManagementObject objMO in objMOC)
                    {
                        var caption = (string)objMO["Caption"];
                        if(caption.Contains("VirtualBox Host-Only Ethernet Adapter"))
                        {
                            try
                            {
                                act(objMO);
                            }
                            catch (Exception e)
                            {
                                Log(e.ToString());
                            }
                        }
                    }
                }
            }
        }
    }
}
