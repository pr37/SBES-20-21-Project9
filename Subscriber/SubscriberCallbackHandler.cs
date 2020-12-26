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
            List<Alarm> alarms = new List<Alarm>();
            foreach (byte[] sign in signedEncryptedAlarms.Keys)
            {
                alarms.Add(AESInECB.DecryptAlarm(signedEncryptedAlarms[sign], SecretKey.LoadKey(secretKeyPath)));
            }

            foreach (Alarm alarm in alarms)
            {
                if (AlarmValidator.Validate(alarm)) //TODO izmeni ovaj validator da prima sign i da validira ili ga samo ovde validiraj
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
