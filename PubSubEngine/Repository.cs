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
        //public static List<Alarm> alarms = new List<Alarm>();
        public static Dictionary<byte[], Alarm> signedAlarms = new Dictionary<byte[], Alarm>(); //sign,Alarm
    }
}
