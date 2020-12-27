using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace Contracts
{
    [ServiceContract]
    public interface IPublish
    {
        [OperationContract]
        void Publish(byte[] encryptedAlarm, byte[] sign, int processId); 
    }
}
