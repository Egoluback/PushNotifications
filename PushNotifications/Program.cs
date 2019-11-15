using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Security.Principal;

using Microsoft.Win32;

namespace PushNotifications
{
    class AutoRun {
        private string name;

        public AutoRun(String name) {
            this.name = name;
        }
        public bool Run(bool autorun)
            {
                string ExePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                RegistryKey reg;
                reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
                try
                {
                    if (autorun) reg.SetValue(name, ExePath);
                    else reg.DeleteValue(name);

                    reg.Close();
                }
                catch
                {
                    return false;
                }
                return true;
            }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                AutoRun autoRun = new AutoRun("PushNotifications");
                autoRun.Run(true);
            }
            catch (Exception e) {
                Console.WriteLine("Catched exception:" + e.Message + "... I'll continue the work.");
            }

            String host = System.Net.Dns.GetHostName();
            System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];

            String userName = WindowsIdentity.GetCurrent().Name;


            string result;
            string url = @"http://wirepusher.com/send?id=Xh3bmpgWB&title=" + ip.ToString() + "%20launched&message=Pay%20attention%20:" + "%20" + userName + "%20launched%20PC&type=RunComputer";

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return;
            }
        }
    }
}
