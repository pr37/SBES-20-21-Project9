using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Manager;
using Models;
using SecurityManager;

namespace Publisher
{
    public class Publisher : ChannelFactory<IPublish>, IPublish, IDisposable
    {
		IPublish factory;
		public int PublishingInterval { get; set; } //milliseconds
		public string signCertCN { get; set; }

        public Publisher(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			/// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
			/// .NET WindowsIdentity class provides information about Windows user running the given process
			signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

        internal void StartPublishing()
        {
			Alarm alarm;
            while(true)
            {
				int risk = GenerateRisk();
				AlarmMessagesTypes msg = GenerateMessageType(risk);
				alarm = new Alarm(DateTime.Now, risk, msg);

				this.Publish(alarm,CreateSignature(alarm.Message,signCertCN));

				Thread.Sleep(PublishingInterval);
            }
        }

        private AlarmMessagesTypes GenerateMessageType(int risk)
        {
            if (risk < (Alarm.MaxRisk / 3))
            {
				return AlarmMessagesTypes.LowPrio;
            }
			else if (risk > (Alarm.MaxRisk * 2 / 3 ))
            {
				return AlarmMessagesTypes.HighPrio;
			}
			else
            {
				return AlarmMessagesTypes.StandardPrio;
			}
        }

        private int GenerateRisk()
        {
			Random rand = new Random(Guid.NewGuid().GetHashCode());

			return rand.Next(Alarm.MinRisk, Alarm.MaxRisk + 1); //bc max value is exclusive

		}

        public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void Publish(Alarm alarm, byte[] signature)
        {
			try
			{
				factory.Publish(alarm,signature);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Publish] ERROR = {0}", e.Message);
			}
		}

		public byte[] CreateSignature(string message, string signCertCN)
        {
			X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
					StoreLocation.LocalMachine, signCertCN);

			return DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSign);
		}
    }
}
