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

        public static bool finished = false;
        public static DateTime lastPublisher = default;
        // public static DateTime newPublisher = default;
        public static bool thereIsNewPub = false;
        public static bool isFirstTime = true;
        public SubscriberCallbackHandler()
        {
            _alarms = new List<Alarm>();
            Process thisProces = Process.GetCurrentProcess();
            writer = new Writer("SubscriberLogFile_"+thisProces.Id.ToString()+".txt");
        }

        public void PushTopic(Dictionary<byte[],byte[]> signedEncryptedAlarms, DateTime lastKnownPublisher)
        {
            /*
            Dictionary<byte[], Alarm> alarms = new Dictionary<byte[], Alarm>();
            
           
            foreach (byte[] sign in signedEncryptedAlarms.Keys)
            {
                alarms.Add(AESInECB.DecryptAlarm(signedEncryptedAlarms[sign], SecretKey.LoadKey(secretKeyPath)));
            }
            */
            if (!finished) { return; }
            foreach (var signedAlarmPair in signedEncryptedAlarms)
            {
                byte[] encryptedAlarm = signedAlarmPair.Value;
                byte[] signature = signedAlarmPair.Key;
                Alarm alarm = AESInECB.DecryptAlarm(encryptedAlarm, SecretKey.LoadKey(secretKeyPath));
               

                if (AlarmValidator.Validate(alarm, encryptedAlarm, signature)) 
                {
                    _alarms.Add(alarm);
                    Console.WriteLine($"Subscriber received {alarm.ToString()}");
                    writer.Write(alarm.ToString());
                }
                else
                {
                    Console.WriteLine($"Alarm: {alarm} ; rejected.");
                }
            }
            if(lastKnownPublisher!=lastPublisher && !isFirstTime)
            {
                thereIsNewPub = true;
            //    isFirstTime = false;
                lastPublisher = lastKnownPublisher;
            }
            isFirstTime = false;
        }

        public void SendBackPublishers(List<int> publishers)
        {
         //   lastPublisher = DateTime.Now;
            Console.WriteLine("Here's the list of publishers, pick one by choosing its index number: ");
            for(int i = 0; i < publishers.Count; i++)
            {
                Console.WriteLine(" "+ i.ToString() + " : " + publishers[i].ToString());
            }
            while(true)
            {
                Console.WriteLine("Enter index (enter x if you are finnished)");
                string ent = Console.ReadLine();
                if (ent.Equals("x"))
                {
                    finished = true;
                    break;
                }
                int index = Int32.Parse(ent); //TRY PARSE
                
                SubscriberCallbackProxy.subscribedPublishers.Add(publishers[index]);
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
