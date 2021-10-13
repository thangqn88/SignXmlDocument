using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace VerifySignedXml.Api.Models
{
    public class ResultModel
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public string FilePath { get; set; }

        public X509Certificate2 X509Certificate { get; set; }
    }
}
