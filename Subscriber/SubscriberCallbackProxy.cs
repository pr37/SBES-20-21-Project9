using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Models;
using SecurityManager;

namespace Subscriber
{
    class SubscriberCallbackProxy : DuplexChannelFactory<ISubscribe>, ISubscribe, IDisposable
    {
        ISubscribe factory;

        public SubscriberCallbackProxy(InstanceContext instanceContext, NetTcpBinding binding, EndpointAddress address) : 
            base(instanceContext, binding, address )
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name); //TODO vrati
            //string cltCertCN = "Subscriber";


            //signCertCN = cltCertCN;


            /// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
            /// .NET WindowsIdentity class provides information about Windows user running the given process
            //signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);



            factory = this.CreateChannel();
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            /*
            if (callbackFactory != null)
            {
                callbackFactory = null;
            }
            */

            this.Close();
        }

        /*

        public void PushTopic(List<Alarm> alarms)
        {
            try
            {
                callbackFactory.PushTopic(alarms);
                //Console.WriteLine($"Subrscribed to topic [{from}-{to}]");
            }
            catch (Exception e)
            {
                Console.WriteLine("[PushTopic] ERROR = {0}", e.Message);
            }
        }
        */

        public void Subscribe(byte[] encryptedFrom, byte[] encryptedTo)
        {
            try
            {
                factory.Subscribe(encryptedFrom, encryptedTo);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Subscribe] ERROR = {0}", e.Message);
            }
        }
    }
}
