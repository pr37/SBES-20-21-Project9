using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Models;

namespace Publisher
{
    public class Publisher : ChannelFactory<IPublish>, IPublish, IDisposable
    {
		IPublish factory;
		public int PublishingInterval { get; set; } //milliseconds

        public Publisher(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			//string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

			//this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
			//this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			//this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			//this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

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

				this.Publish(alarm);

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

        public void Publish(Alarm alarm)
        {
			try
			{
				factory.Publish(alarm);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Publish] ERROR = {0}", e.Message);
			}
		}
    }
}
