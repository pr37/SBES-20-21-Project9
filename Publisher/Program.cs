using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO CERTS

            NetTcpBinding binding = new NetTcpBinding();
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Publishers"));

            using (Publisher proxy = new Publisher(binding, address))
            {

                Console.WriteLine("Publisher online.Define publishing interval(integer, milliseconds): ");
                string input = Console.ReadLine();
                int interval;
                while (!Int32.TryParse(input, out interval))
                {
                    Console.WriteLine("Enter a proper integer: ");
                    input = Console.ReadLine();
                }

                proxy.PublishingInterval = interval;
                proxy.StartPublishing();

                Console.WriteLine("Communication aborted. Press <enter> to continue ...");
                Console.ReadLine();
            }
        }
    }
}
