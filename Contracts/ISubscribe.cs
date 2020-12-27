using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Contracts
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ISubscribeCallback))]
    public interface ISubscribe
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(byte[] encryptedFrom, byte[] encryptedTo, List<int> publishers);
        [OperationContract(IsOneWay = true)]
        void ConnectToPublishers(); //SEE ALL PUBLISHERS

    }

    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISubscribeCallback
    {
        [OperationContract(IsOneWay = true)]
       // void PushTopic(List<byte[]> encryptedAlarms);
        void PushTopic(Dictionary<byte[],byte[]> signedEncryptedAlarms, DateTime lastKnownPublisher);
        [OperationContract(IsOneWay = true)]
        void SendBackPublishers(List<int> publishers);
    }
}
