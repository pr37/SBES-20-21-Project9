using Contracts;
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
          //  SendDelta();
        }

        public void SendDelta(List<int> publishers)
        {
            while (true)
            {
                Dictionary<byte[], Alarm> data = new Dictionary<byte[], Alarm>();
                Dictionary<byte[], Alarm> repoData_copy = new Dictionary<byte[], Alarm>(Repository.signedAlarms);
                foreach (var pub in publishers)
                {
                    data = repoData_copy.Where(
                        x => x.Value.Risk >= From && x.Value.Risk <= To && x.Value.CreationTime > Timestamp && x.Value.Publisher == pub).ToDictionary(x => x.Key, x => x.Value);
                    if (data.Count != 0)
                    {
                        Timestamp = DateTime.Now;

                        // List<byte[]> encryptedAlarms = new List<byte[]>();
                        Dictionary<byte[], byte[]> signedEncryptedAlarms = new Dictionary<byte[], byte[]>();

                        foreach (byte[] key in data.Keys)
                        {
                            signedEncryptedAlarms.Add(key, AESInECB.EncryptAlarm(data[key], SecretKey.LoadKey(secretKeyPath)));
                        }

                        this.Callback.PushTopic(signedEncryptedAlarms,PubService.lastConnectedPublisher);
                        data.Clear();
                    }
                    Thread.Sleep(600);

                    //provera da li je dosao novi pablisher

                }
                
            }
        }

        public void Subscribe(byte[] encryptedFrom, byte[] encryptedTo, List<int> publishers)
        {
            Timestamp = DateTime.Now;
            int from = AESInECB.DecryptInteger(encryptedFrom, SecretKey.LoadKey(secretKeyPath));
            int to = AESInECB.DecryptInteger(encryptedTo, SecretKey.LoadKey(secretKeyPath));
            
            From = from; To = to;
            //List<Alarm> data = Repository.alarms.FindAll(x => x.Risk > from && x.Risk < to);
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
            SendDelta(publishers);
        }

        public void ConnectToPublishers()
        {
            this.Callback.SendBackPublishers(Repository.publishers);
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
