﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Threading;
using SymmetricAlgorithmAES;
using SecurityManager;

namespace PubSubEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SubService : ISubscribe
    {
        private static readonly string secretKeyPath = "../../../Models/secretKey.txt";

        public int From { get; set; }
        public int To { get; set; }
        public DateTime Timestamp { get; set; }
        public void Subscribe(int from, int to)
        {
            Timestamp = DateTime.Now;
            From = from; To = to;
            //List<Alarm> data = Repository.alarms.FindAll(x => x.Risk > from && x.Risk < to);
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
            SendDelta();
        }

        public void SendDelta()
        {
            while (true)
            {
                List<Alarm> data = Repository.alarms.FindAll(x => x.Risk >= From && x.Risk <= To && x.CreationTime > Timestamp);
                if (data.Count != 0)
                {
                    Timestamp = DateTime.Now;

                    List<byte[]> encryptedAlarms = new List<byte[]>();
                    foreach(Alarm alarm in data)
                    {
                        encryptedAlarms.Add(AESInECB.EncryptAlarm(alarm, SecretKey.LoadKey(secretKeyPath)));
                    }

                    this.Callback.PushTopic(encryptedAlarms);
                    data.Clear();
                }
                Thread.Sleep(50);
            }
        }

        public void Subscribe(byte[] encryptedFrom, byte[] encryptedTo)
        {
            Timestamp = DateTime.Now;
            int from = AESInECB.DecryptInteger(encryptedFrom, SecretKey.LoadKey(secretKeyPath));
            int to = AESInECB.DecryptInteger(encryptedTo, SecretKey.LoadKey(secretKeyPath));
            
            From = from; To = to;
            //List<Alarm> data = Repository.alarms.FindAll(x => x.Risk > from && x.Risk < to);
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
            SendDelta();
        }

        ISubscribeCallback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<ISubscribeCallback>();
            }
        }
    }
}
