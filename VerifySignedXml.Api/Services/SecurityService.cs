using VerifySignedXml.Api.Models;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace VerifySignedXml.Api.Services
{
    public static class SecurityService
    {
        public static ResultModel CheckSignedXmlDoc(string filePath)
        {
            var resultModel = new ResultModel
            {
                FilePath = filePath
            };
            try
            {

                // Create a new CspParameters object to specify
                // a key container.
                CspParameters cspParams = new CspParameters
                {
                    KeyContainerName = "XML_DSIG_RSA_KEY"
                };

                // Create a new RSA signing key and save it in the container.
                RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

                // Create a new XML document.
                XmlDocument xmlDoc = new XmlDocument
                {

                    // Load an XML file into the XmlDocument object.
                    PreserveWhitespace = true
                };

                xmlDoc.Load(filePath);



                Console.WriteLine("Verifying signature...");
                var result = SecurityService.VerifyXml(xmlDoc, rsaKey);

                // Display the results of the signature verification to
                // the console.
                if (result.Item1)
                {
                    resultModel.Status = "Passed";
                    resultModel.Message = "The XML signature is valid.";
                    resultModel.X509Certificate = result.Item2;
                }
                else
                {
                    resultModel.Status = "Failed";
                    resultModel.Message = "The XML signature is not valid.";
                }
            }
            catch (Exception e)
            {
                resultModel.Status = "Failed";
                resultModel.Message = string.Format("Exception: {0}", e.Message);
            }

            return resultModel;
        }


        // Verify the signature of an XML file against an asymmetric
        // algorithm and return the result.
        public static Tuple<bool, X509Certificate2> VerifyXml(XmlDocument xmlDoc, RSA key)
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

            return Tuple.Create(passes, dcert2);
        }
    }

}
