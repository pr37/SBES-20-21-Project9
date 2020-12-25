using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "PubSubEngine";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Publishers"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (Publisher proxy = new Publisher(binding, address))
            {

                Console.WriteLine("Publisher online.Define publishing interval(integer, milliseconds): ");
                string input = Console.ReadLine();
                int interval;
                while (!Int32.TryParse(input, out interval))
                {
                    Console.WriteLine("Enter a proper integer: ");
                    input = Console.ReadLine();
                }

                proxy.PublishingInterval = interval;
                proxy.StartPublishing();

                Console.WriteLine("Communication aborted. Press <enter> to continue ...");
                Console.ReadLine();
            }
        }
    }
}
