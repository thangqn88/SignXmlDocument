using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignXmlDocument.Models;
using SignXmlDocument.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;

namespace SignXmlDocument.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(ResultModel model)
        {
            return View(model);
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> FileUpload(List<IFormFile> files)
        {
            ResultModel checkResult = new ResultModel();

            var filePaths = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    //var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
                    string filePath = "Uploaded/" + file.FileName;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }
            }

            if (filePaths.Count > 0)
            {

                checkResult = CheckXml(filePaths[0]);
            }

            return View("Index", checkResult);
        }
        private ResultModel CheckXml(string filePath)
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
                bool result = SecurityService.VerifyXml(xmlDoc, rsaKey);

                // Display the results of the signature verification to
                // the console.
                if (result)
                {
                    resultModel.Status = "Successed";
                    resultModel.Message = "The XML signature is valid.";
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
