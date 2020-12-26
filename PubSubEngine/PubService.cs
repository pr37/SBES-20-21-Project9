using Contracts;
using Models;
using SecurityManager;
using SymmetricAlgorithmAES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class PubService : IPublish
    {
        private static readonly string secretKeyPath = "../../../Models/secretKey.txt";

        public string clientName { get; set; }
        public string clientNameSign { get; set; }
        public X509Certificate2 certificate { get; set; }
        public PubService()
        {
            //kad je u pitanju autentifikacija putem Sertifikata
            clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            clientNameSign = clientName + "_sign";
            //certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);
        }


        public void Publish(byte[] encryptedAlarm, byte[] sign)
        {
            /*
            if (DigitalSignature.Verify(alarm.Message, HashAlgorithm.SHA1, signature, certificate))
            {
                Console.WriteLine("Sign is valid for: "+ alarm);
                Repository.alarms.Add(alarm);
            }
            else
            {
                Console.WriteLine("Sign is INVALID for: "+ alarm);
            }
            */
            Alarm alarm = AESInECB.DecryptAlarm(encryptedAlarm, SecretKey.LoadKey(secretKeyPath));
            Repository.alarms.Add(alarm);
            Console.WriteLine(alarm);
        }
    }
}
