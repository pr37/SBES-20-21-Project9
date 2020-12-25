using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public class ServiceHostHelper
    {
        public ServiceHost host { get; set; }
        public ServiceHostHelper(string address, Type serviceType, Type contractType, string certName)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            host = new ServiceHost(serviceType);
            host.AddServiceEndpoint(contractType, binding, address);

            CertificateHelper certificateHelper = new CertificateHelper();
            certificateHelper.SetCertificate(host,certName);

            OpenService();
        }

        public void OpenService()
        {
            try
            {
                host.Open();
                Console.WriteLine("Service is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
