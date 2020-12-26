using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    public static class ServiceHostHelper
    {

        public static ServiceHost PrepareHost(string address, Type serviceType, Type contractType, string certName) {

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            ServiceHost host = new ServiceHost(serviceType);
            host.AddServiceEndpoint(contractType, binding, address);

            CertificateHelper certificateHelper = new CertificateHelper();
            certificateHelper.SetCertificate(host, certName);

            return host;
        }


    }
}
