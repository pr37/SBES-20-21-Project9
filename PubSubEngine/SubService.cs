using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Threading;

namespace PubSubEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SubService : ISubscribe
    {
        public int From { get; set; }
        public int To { get; set; }
        public DateTime Timestamp { get; set; }
        public void Subscribe(int from, int to)
        {
            Timestamp = DateTime.Now;
            From = from; To = to;
            //List<Alarm> data = Repository.alarms.FindAll(x => x.Risk > from && x.Risk < to);
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
            Thread thread = new Thread(new ThreadStart(SendDelta));
        }

        public void SendDelta()
        {
            while (true)
            {
                List<Alarm> data = Repository.alarms.FindAll(x => x.Risk > From && x.Risk < To && x.CreationTime > Timestamp);
                if (data.Count != 0)
                {
                    Timestamp = DateTime.Now;
                    this.Callback.PushTopic(data);
                    data.Clear();
                }
                Thread.Sleep(3000);
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
