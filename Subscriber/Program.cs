using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SecurityManager;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "PubSubService";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Subscribers"),
                                      new X509CertificateEndpointIdentity(srvCert));

            InstanceContext instanceContext = new InstanceContext(new SubscriberCallbackHandler());
            using (SubscriberCallbackProxy proxy = new SubscriberCallbackProxy(instanceContext ,binding, address))
            {
                Console.WriteLine("Define minimum and maximum risk of iterest(integer_integer):");
                string input = Console.ReadLine();
                string[] ints = input.Split(' ');
                int minRisk, maxRisk;
                while (ints.Length != 2 || !Int32.TryParse(ints[0], out minRisk) || !Int32.TryParse(ints[1], out maxRisk))
                {
                    Console.WriteLine("Enter 2 proper integers separeted by a space: ");
                    input = Console.ReadLine();
                    ints = input.Split(' ');
                }

                proxy.Subscribe(minRisk, maxRisk);

                Console.ReadLine();
                Console.ReadLine();
            }
        }

    }
}
