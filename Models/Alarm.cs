using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [DataContract]
    public enum AlarmMessagesTypes
    {
        [EnumMember]
        LowPrio = 0,

        [EnumMember]
        StandardPrio,

        [EnumMember]
        HighPrio
    }

    [DataContract]
    [Serializable]
    public class Alarm
    {
        [DataMember]
        public static int MaxRisk = 100;

        [DataMember]
        public static int MinRisk = 1;


        [DataMember]
        public DateTime CreationTime { get;  set; }


        [DataMember]
        private int _risk;


        [DataMember]
        public int Risk {
            get { return _risk; }
             set
            {
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Risk values are in the scope of [1-100]");
                }
                _risk = value;
            }
        }

        [DataMember]
        public string Message { get;  set; }


        public Alarm()
        {

        }

        public Alarm(DateTime creation, int risk, AlarmMessagesTypes type)
        {
            CreationTime = creation;
            Risk = risk;
            ResourceManager rM = new ResourceManager(typeof(AlarmMessages).FullName, Assembly.GetExecutingAssembly());
            Message = rM.GetString(type.ToString());
        }

        public override string ToString()
        {
            return string.Format($"[{this.CreationTime}] : Risk:{this.Risk} : Additional Message:{this.Message}");
        }

    }
}
