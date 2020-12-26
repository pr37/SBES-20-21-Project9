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
		/// <summary>
		/// Implementation of a custom certificate validation on the client side.
		/// Client should consider certificate valid if the given certifiate is not self-signed.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		public override void Validate(X509Certificate2 certificate)
		{
			if (certificate.Subject.Equals(certificate.Issuer))
			{
				throw new Exception("Certificate is self-issued.");
			}

			DateTime expirationDate = DateTime.Parse(certificate.GetExpirationDateString());
			if (expirationDate < DateTime.Now)
			{
				throw new Exception($"Certificate expired on [{expirationDate}]");
			}
		}
	}
}
