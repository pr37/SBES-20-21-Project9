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
        public static DateTime lastConnectedPublisher = default;

        public PubService()
        {

        }


        public void Publish(byte[] encryptedAlarm, byte[] sign, int processId)
        {
            
            if (DigitalSignature.Verify(encryptedAlarm, HashAlgorithm.SHA1, sign, GetSignatureCertificate()))
            {
                Alarm alarm = AESInECB.DecryptAlarm(encryptedAlarm, SecretKey.LoadKey(secretKeyPath));
                Repository.signedAlarms.Add(sign,alarm);
                if (!Repository.publishers.Contains(processId))
                {
                    Repository.publishers.Add(processId);
                    lastConnectedPublisher = DateTime.Now;
                }
                Console.WriteLine(alarm);
            }
            else
            {
                Console.WriteLine("Signature is INVALID for");
            }
            

        }

        private X509Certificate2 GetSignatureCertificate() { 

            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string clientNameSign = clientName + "_sign";

            X509Certificate2 certificate = null; //the method wont ever return null due to catch bellow
            try
            {
                certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientNameSign);
                return certificate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }

            
        }
    }
}
