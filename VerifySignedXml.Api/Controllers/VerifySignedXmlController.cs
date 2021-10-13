using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VerifySignedXml.Api.Models;
using VerifySignedXml.Api.Services;

namespace VerifySignedXml.Api.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class VerifySignedXmlController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to the world of .Net");

        }


        [HttpPost]
        public async Task<IActionResult> Post(
         IFormFile file,
         CancellationToken cancellationToken)
        {
            var result = new ResultModel();
            if (CheckIfXmlFile(file))
            {
                var filePath = await WriteFile(file);

                if (!string.IsNullOrEmpty(filePath))
                {
                    result = SecurityService.CheckSignedXmlDoc(filePath);

                }
            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }

            return Ok(result);
        }

        private bool CheckIfXmlFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".xml"); // Change the extension based on your need
        }

        private async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            string path = string.Empty;
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            path = Path.Combine(Directory.GetCurrentDirectory(), "Upload",
               fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            return path;

        }
    }
}
