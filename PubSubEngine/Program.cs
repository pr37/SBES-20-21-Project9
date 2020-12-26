using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
            //string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string srvCertCN = "PubSubService"; 


            string addressPub = "net.tcp://localhost:9999/Publishers";
            ServiceHost pubHost = ServiceHostHelper.PrepareHost(addressPub, typeof(PubService), typeof(IPublish), srvCertCN);

            string addressSub = "net.tcp://localhost:9999/Subscribers";
            ServiceHost subHost = ServiceHostHelper.PrepareHost(addressSub, typeof(SubService), typeof(ISubscribe), srvCertCN);

            OpenService(subHost, pubHost);
        }


        public static void OpenService(ServiceHost subHost, ServiceHost pubHost)
        {
            try
            {
                pubHost.Open();
                Console.WriteLine("Publisher host has started.");
                subHost.Open();
                Console.WriteLine("Subscriber host has started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                pubHost.Close();
                subHost.Close();
            }
        }
    }
}
