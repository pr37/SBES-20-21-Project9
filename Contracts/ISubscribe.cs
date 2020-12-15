using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ISubscribe
    {
        [OperationContract]
        void Subscribe(int from, int to);

    }
}
