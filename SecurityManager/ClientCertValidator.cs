using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace SecurityManager
{
	public class ClientCertValidator : X509CertificateValidator
	{
		//klijent verifikuje servis, servis TREBA da bude issued sam od sebe
		public override void Validate(X509Certificate2 certificate)
		{
			Console.WriteLine($"CLIENT VALIDATING {certificate.Subject}:{certificate.Issuer}");

			if (!certificate.Subject.Equals(certificate.Issuer))
			{
				throw new Exception($"Certificate {certificate.Subject} is self-issued.");
			}

			DateTime expirationDate = DateTime.Parse(certificate.GetExpirationDateString());
			if (expirationDate < DateTime.Now)
			{
				throw new Exception($"Certificate expired on [{expirationDate}]");
			}


			Console.WriteLine("CLIENT VALIDATING SUCCESS");
		}
	}
}
