using Models;
using SecurityManager;
using SymmetricAlgorithmAES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public static class AlarmValidator
    {
        public static bool Validate(Alarm alarm, byte[] encryptedAlarm , byte[] signature) 
        {
            return ValidateAlarm(alarm) && ValidateSignature(encryptedAlarm, signature);

        }

        private static bool ValidateAlarm(Alarm alarm)
        {
            if (alarm.Risk < Alarm.MinRisk || alarm.Risk > Alarm.MaxRisk) return false;

            if (alarm.Message == null || alarm.Message == string.Empty)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateSignature(byte[] encryptedAlarm, byte[] signature)
        {
            return DigitalSignature.Verify(encryptedAlarm, HashAlgorithm.SHA1, signature, GetSignatureCertificate());
        }

        private static X509Certificate2 GetSignatureCertificate()
        {

            //string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name); this would give name of the service
            //but the service passess on Publishers data therefore i'll hardcode Publishers name
            string clientNameSign = "Publisher_sign";

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
