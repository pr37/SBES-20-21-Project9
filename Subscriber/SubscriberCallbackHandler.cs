using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Writer writer;

        public SubscriberCallbackHandler()
        {
            _alarms = new List<Alarm>();
            Process thisProces = Process.GetCurrentProcess();
            writer = new Writer("SubscriberLogFile_"+thisProces.Id.ToString()+".txt");
        }


        public void PushTopic(List<Alarm> alarms)
        {
            foreach(Alarm alarm in alarms)
            {
                if (AlarmValidator.Validate(alarm))
                {
                    _alarms.Add(alarm);
                    Console.WriteLine($"Subscriber received {alarm.ToString()}");
                    writer.Write(alarm.ToString());
                }
                else
                {
                    Console.WriteLine($"Alarm: {alarm.ToString()} ; rejected.");
                }
            }
        }

    }
}
