using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Models;

namespace Subscriber
{
    class SubscriberCallbackProxy : DuplexChannelFactory<ISubscribe>, ISubscribe, IDisposable
    {
        ISubscribe factory;

        public SubscriberCallbackProxy(InstanceContext instanceContext, NetTcpBinding binding, EndpointAddress address) : 
            base(instanceContext, binding, address )
        {
            factory = this.CreateChannel();
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            /*
            if (callbackFactory != null)
            {
                callbackFactory = null;
            }
            */

            this.Close();
        }

        /*

        public void PushTopic(List<Alarm> alarms)
        {
            try
            {
                callbackFactory.PushTopic(alarms);
                //Console.WriteLine($"Subrscribed to topic [{from}-{to}]");
            }
            catch (Exception e)
            {
                Console.WriteLine("[PushTopic] ERROR = {0}", e.Message);
            }
        }
        */

        public void Subscribe(int from, int to)
        {
            try
            {
                factory.Subscribe(from, to);
                Console.WriteLine($"Subrscribed to topic [{from}-{to}]");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Subscribe] ERROR = {0}", e.Message);
            }
        }
    }
}
