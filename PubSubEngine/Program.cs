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
            string srvCertCN = "PubSubEngine"; //POKRENI SA USER PubSubEngine 
            
            string addressPub = "net.tcp://localhost:9999/Publishers";
            ServiceHostHelper pubHelper = new ServiceHostHelper(addressPub, typeof(PubService), typeof(IPublish), srvCertCN);

            string addressSub = "net.tcp://localhost:9999/Subscribers";
            ServiceHostHelper subHelper = new ServiceHostHelper(addressSub, typeof(SubService), typeof(ISubscribe), srvCertCN);

        }
    }
}
