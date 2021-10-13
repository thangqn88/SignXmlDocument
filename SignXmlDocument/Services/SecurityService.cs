using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SignXmlDocument.Services
{
    public static class SecurityService
    {

        // Verify the signature of an XML file against an asymmetric
        // algorithm and return the result.
        public static Boolean VerifyXml(XmlDocument xmlDoc, RSA key)
        {
            bool passes = false;

            // Check arguments.
            if (xmlDoc == null)
                throw new ArgumentException("xmlDoc");
            if (key == null)
                throw new ArgumentException("key");



            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");

            XmlNodeList certificates = xmlDoc.GetElementsByTagName("X509Certificate");
            X509Certificate2 dcert2 = new X509Certificate2(Convert.FromBase64String(certificates[0].InnerText));



            // Throw an exception if no signature was found.
            if (nodeList.Count <= 0)
            {
                throw new CryptographicException("Verification failed: No Signature was found in the document.");
            }

            foreach (XmlElement element in nodeList)
            {
                signedXml.LoadXml(element);
                passes = signedXml.CheckSignature(dcert2, true);
            }

            return passes;
        }
    }
   
}
