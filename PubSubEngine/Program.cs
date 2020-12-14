using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string addressPub = "net.tcp://localhost:9999/Publishers";
            ServiceHost hostPub = new ServiceHost(typeof(PubService));
            hostPub.AddServiceEndpoint(typeof(IPublish), binding, addressPub);

            //binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string addressSub = "net.tcp://localhost:9999/Subscribers";
            ServiceHost hostSub = new ServiceHost(typeof(SubService));
            hostSub.AddServiceEndpoint(typeof(ISubscribe), binding, addressSub);

            try
            {
                hostPub.Open();
                hostSub.Open();
                Console.WriteLine("PubService and SubService are started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                hostPub.Close();
                hostSub.Close();
            }
        }
    }
}
