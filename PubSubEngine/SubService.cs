﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace PubSubEngine
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SubService : ISubscribe
    {

        public void Subscribe(int from, int to)
        {
            Console.WriteLine($"Subccriber XYZ subcribed to [{from}-{to}]");
            this.Callback.PushTopic(new List<Alarm>()); //TEST
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
