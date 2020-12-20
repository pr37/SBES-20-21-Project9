using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public static class AlarmValidator
    {
        public static bool Validate(Alarm alarm) //how can an alarm be invalid?
        {
            if (alarm.Risk < Alarm.MinRisk || alarm.Risk > Alarm.MaxRisk) return false;

            if (alarm.Message == null || alarm.Message == string.Empty)
            {
                return true;
            }

            return true;
        }
    }
}
