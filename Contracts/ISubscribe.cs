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
        void Subscribe(byte[] encryptedFrom, byte[] encryptedTo);

    }

    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface ISubscribeCallback
    {
        [OperationContract(IsOneWay = true)]
        void PushTopic(List<byte[]> encryptedAlarms);
    }
}
