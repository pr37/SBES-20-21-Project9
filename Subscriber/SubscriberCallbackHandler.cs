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
        private List<Alarm> _alarms;

        public SubscriberCallbackHandler()
        {
            _alarms = new List<Alarm>();
        }


        public void PushTopic(List<Alarm> alarms)
        {
            foreach(Alarm alarm in alarms)
            {
                _alarms.Add(alarm);
                Console.WriteLine($"Subscriber received {alarm.ToString()}");
            }
        }

    }
}
