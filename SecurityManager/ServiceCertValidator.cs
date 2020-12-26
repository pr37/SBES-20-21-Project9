using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace SecurityManager
{
	public class ServiceCertValidator : X509CertificateValidator
	{
		//servis validira klijenta, klijent treba da bude issued od servisa
		/// <param name="certificate"> certificate to be validate </param>
		public override void Validate(X509Certificate2 certificate)
		{
			/// This will take service's certificate from storage
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine,
				//Formatter.ParseName(WindowsIdentity.GetCurrent().Name)); TODO vrati
				"PubSubService");

			Console.WriteLine($"SERVICE VALIDATING {certificate.Subject}:{certificate.Issuer} WITH {srvCert.Subject}:{srvCert.Issuer}");
		

			if (!certificate.Issuer.Equals(srvCert.Issuer))
			{
				throw new Exception("Certificate is not from the valid issuer.");
			}

			DateTime expirationDate = DateTime.Parse(certificate.GetExpirationDateString());
			if (expirationDate < DateTime.Now)
            {
				throw new Exception($"Certificate expired on [{expirationDate}]");
            }

			Console.WriteLine("SERVICE VALIDATING SUCCESS");
		}
	}
}
