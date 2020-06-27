using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileUpload.Models;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Drive API .NET Quickstart";
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost(IFormFile fileUpload)
        {
            if (Request.Method == "POST")
            {

                UserCredential credential;
                
                using (var stream =
                new FileStream("client.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                   GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user4",
                    CancellationToken.None,
                    new FileDataStore("MyAppsToken")).Result;

                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                uploadFile(service, Path.Combine(Directory.GetCurrentDirectory(), "assignment.pdf"));

                ViewBag.Message = "File Uploaded Successfully!!!";
            }
            return View();
        }

        public Google.Apis.Drive.v3.Data.File uploadFile(DriveService _service, string _uploadFile)
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "Assignment";
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, "");
                    request.SupportsTeamDrives = true;
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception)
                {

                    return null;
                }
            }
            else
            {

                return null;
            }
        }


    }
}
