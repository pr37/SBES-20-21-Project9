using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "SecurityManager";
        const string LogName = "DatabaseInput";

        static Audit()
        {
            // Creating customLog handle
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void DatabaseInput(DateTime timeStamp, string databaseName, string entityID, string digitalSign, string publicKey)
        {
            // TO DO
            if (customLog != null)
            {
                string str = AuditEvents.DatabaseInputSucces;
                string message = String.Format(str, timeStamp, databaseName, entityID, digitalSign, publicKey);

                customLog.WriteEntry(message);
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }        
    }
}
