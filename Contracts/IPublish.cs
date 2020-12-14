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
        void Publish(Alarm alarm); //TODO: ovo treba biti digitalno potpisano
    }
}
