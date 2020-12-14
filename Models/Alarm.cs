using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum AlarmMessagesTypes
    {
        LowPrio = 0,
        StandardPrio,
        HighPrio
    }

    public class Alarm
    {
        public DateTime CreationTime { get; private set; }

        private int _risk;

        public int Risk {
            get { return _risk; }
            private set
            {
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Risk values are in the scope of [1-100]");
                }
                _risk = value;
            }
        }
        public string Message { get; private set; }

        public Alarm(DateTime creation, int risk, AlarmMessagesTypes type)
        {
            CreationTime = creation;
            Risk = risk;
            ResourceManager rM = new ResourceManager(typeof(AlarmMessages).FullName, Assembly.GetExecutingAssembly());
            Message = rM.GetString(type.ToString());
        }

    }
}
