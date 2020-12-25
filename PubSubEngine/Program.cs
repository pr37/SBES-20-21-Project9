using Contracts;
using Manager;
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
		    string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            
            string addressPub = "net.tcp://localhost:9999/Publishers";
            ServiceHost hostPub = new ServiceHost(typeof(PubService));
            hostPub.AddServiceEndpoint(typeof(IPublish), binding, addressPub);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
			hostPub.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostPub.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            hostPub.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
                System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            hostPub.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            hostPub.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string addressSub = "net.tcp://localhost:9999/Subscribers";
            ServiceHost hostSub = new ServiceHost(typeof(SubService));
            hostSub.AddServiceEndpoint(typeof(ISubscribe), binding, addressSub);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
			hostSub.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostSub.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            hostSub.Credentials.ClientCertificate.Authentication.CertificateValidationMode =
            System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            hostSub.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            hostSub.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            try
            {
                hostPub.Open();
                hostSub.Open();
                Console.WriteLine("PubService and SubService are started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                hostPub.Close();
                hostSub.Close();
            }
        }
    }
}
