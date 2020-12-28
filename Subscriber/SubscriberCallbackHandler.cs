using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Models;
using SecurityManager;
using SymmetricAlgorithmAES;

namespace Subscriber
{
    public class SubscriberCallbackHandler : ISubscribeCallback
    {
        private static readonly string secretKeyPath = "../../../Models/secretKey.txt";

        private List<Alarm> _alarms;
        private Writer writer;

        public SubscriberCallbackHandler()
        {
            _alarms = new List<Alarm>();
            Process thisProces = Process.GetCurrentProcess();
            writer = new Writer("SubscriberLogFile_"+thisProces.Id.ToString()+".txt");
        }

        public void PushTopic(Dictionary<byte[],byte[]> signedEncryptedAlarms)
        {
            /*
            Dictionary<byte[], Alarm> alarms = new Dictionary<byte[], Alarm>();

           
            foreach (byte[] sign in signedEncryptedAlarms.Keys)
            {
                alarms.Add(AESInECB.DecryptAlarm(signedEncryptedAlarms[sign], SecretKey.LoadKey(secretKeyPath)));
            }
            */


            foreach (var signedAlarmPair in signedEncryptedAlarms)
            {
                byte[] encryptedAlarm = signedAlarmPair.Value;
                byte[] signature = signedAlarmPair.Key;
                Alarm alarm = AESInECB.DecryptAlarm(encryptedAlarm, SecretKey.LoadKey(secretKeyPath));
               

                if (AlarmValidator.Validate(alarm, encryptedAlarm, signature)) 
                {
                    _alarms.Add(alarm);
                    Console.WriteLine($"Subscriber received {alarm.ToString()}");
                    //string sign = Encoding.Unicode.GetString(signature);
                    string sign = Convert.ToBase64String(signature);
                    string id = Guid.NewGuid().ToString();
                    writer.Write(alarm.ToString(), id, sign, AlarmValidator.GetSignatureCertificate().GetPublicKeyString());
                }
                else
                {
                    Console.WriteLine($"Alarm: {alarm} ; rejected.");
                }
            }
        }

        /*
        public void PushTopic(List<byte[]> encryptedAlarms)
        {
            List<Alarm> alarms = new List<Alarm>();
            foreach(byte[] alarm in encryptedAlarms)
            {
                alarms.Add(AESInECB.DecryptAlarm(alarm, SecretKey.LoadKey(secretKeyPath)));
            }

            foreach (Alarm alarm in alarms)
            {
                if (AlarmValidator.Validate(alarm))
                {
                    _alarms.Add(alarm);
                    Console.WriteLine($"Subscriber received {alarm.ToString()}");
                    writer.Write(alarm.ToString());
                }
                else
                {
                    Console.WriteLine($"Alarm: {alarm.ToString()} ; rejected.");
                }
            }
        } */
    }
}
