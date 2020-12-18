using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Models;

namespace Subscriber
{
    public class SubscriberCallbackHandler : ISubscribeCallback
    {
        public void PushTopic(List<Alarm> alarms)
        {
            throw new NotImplementedException();
        }

    }
}
