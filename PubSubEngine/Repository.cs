using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class Repository
    {
        private static object locker = new object();
        //public static List<Alarm> alarms = new List<Alarm>();
        private static Dictionary<byte[], Alarm> _signedAlarms = new Dictionary<byte[], Alarm>(); //sign,Alarm
        public static Dictionary<byte[], Alarm> signedAlarms { 
            get
            {
                lock(locker)
                {
                    return _signedAlarms;
                }
            } 
            set
            {
                lock (locker)
                {
                    _signedAlarms = value;
                }
            }
        }
    }
}
