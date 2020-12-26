using System;
using System.Collections.Generic;
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

        public SubscriberCallbackHandler()
        {
            _alarms = new List<Alarm>();
        }


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
                }
                else
                {
                    Console.WriteLine($"Alarm: {alarm.ToString()} ; rejected.");
                }
            }
        }
    }
}
