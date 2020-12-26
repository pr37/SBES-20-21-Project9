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
using Models;
using SecurityManager;
using SymmetricAlgorithmAES;

namespace Publisher
{
    public class Publisher : ChannelFactory<IPublish>, IPublish, IDisposable
    {
		IPublish factory;
		public int PublishingInterval { get; set; } //milliseconds


		private static readonly string secretKeyPath = "../../../Models/secretKey.txt";
		//private static readonly string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
		private static readonly string signCertCN = "Publisher" + "_sign";


		public Publisher(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			//string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name); //TODO vrati
			string cltCertCN = "Publisher";


			//signCertCN = cltCertCN;


			/// Define the expected certificate for signing ("<username>_sign" is the expected subject name).
			/// .NET WindowsIdentity class provides information about Windows user running the given process
			//signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

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

				//this.Publish(alarm,CreateSignature(alarm.Message,signCertCN)); // TODO
				try
                {
					byte[] encrytpedAlarm = AESInECB.EncryptAlarm(alarm, SecretKey.LoadKey(secretKeyPath));
					byte[] signature = CreateSignature(encrytpedAlarm, signCertCN);

					this.Publish(encrytpedAlarm, signature);
					Console.WriteLine($"Published: {alarm}");
				}
				catch(Exception e)
                {
					throw new Exception(e.Message);
                }



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

			try
            {
				this.Close();
			}
			catch { }
		}


		public byte[] CreateSignature(byte[] data, string signCertCN)
        {
			X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
					StoreLocation.LocalMachine, signCertCN);

			return DigitalSignature.Create(data, HashAlgorithm.SHA1, certificateSign);
		}

        public void Publish(byte[] encryptedAlarm, byte[] sign)
        {
			try
			{
				factory.Publish(encryptedAlarm, sign);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Publish] ERROR = {0}", e.Message);
				throw new Exception(e.Message);
			}
		}
    }
}
