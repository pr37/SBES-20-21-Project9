using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;
using SecurityManager;

namespace PubSubEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SubService : ISubscribe
    {

        public void Subscribe(int from, int to)
        {
            List<Alarm> testData = new List<Alarm>()
            {
                new Alarm(DateTime.Now, 1 , AlarmMessagesTypes.LowPrio),
                new Alarm(DateTime.Now, 53, AlarmMessagesTypes.StandardPrio)
            };
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");            
            this.Callback.PushTopic(testData);

            try
            {
                Alarm alarm1 = new Alarm(DateTime.Now, 20, AlarmMessagesTypes.LowPrio);

                string keyFile = "SecretKey.txt";            //secret key storage
                ///Generate secret key for appropriate symmetric algorithm and store it to 'keyFile' for further usage
                string eSecretKey = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKey, keyFile);  // storovanje ovog kljuca 

                byte[] encryptedAlarm = SymmetricAlgorithmAES.AESInECB.EncriptAlarm(alarm1, eSecretKey);

                Alarm decryptedAlarm = SymmetricAlgorithmAES.AESInECB.DecryptAlarm(encryptedAlarm, SecretKey.LoadKey(keyFile));

                Console.WriteLine("Enkriptovano-dektiptovani alarm: " + decryptedAlarm.CreationTime + ", risk: " + decryptedAlarm.Risk + ", msg: " + decryptedAlarm.Message);
            }
            catch(Exception e){
                Console.WriteLine(e);
            }



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
