using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



// https://www.c-sharpcorner.com/article/how-to-file-upload-in-angular-5-with-asp-net-core-in-vs2017/

namespace PortfolioNetCore.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        readonly string _contentDirectoryPath = "";

        public FileController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _contentDirectoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
        }

        [HttpPost, DisableRequestSizeLimit] //RequestSizeLimit(500)
        public ActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                string newPath = _contentDirectoryPath;
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fullPath = Path.Combine(newPath, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Json("Upload Successful.");
            }
            catch (System.Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }
        }
                
        [HttpGet("[action]")]
        public IActionResult Download()
        {
            string fileName = "Valami.json";
            var filePath = Path.Combine(_contentDirectoryPath, fileName);

            //if (!System.IO.File.Exists(filePath))
            //{
            //    return HttpNotFound($"File does not exist: {id}");
            //}

            //var adminClaim = User.Claims.FirstOrDefault(x => x.Type == "role" && x.Value == "securedFiles.admin");
            //if (_securedFileProvider.HasUserClaimToAccessFile(id, adminClaim != null))
            //{
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            // return new FileContentResult(fileContents, "application/octet-stream");
            //}
            return Ok(new FileContentResult(fileContents, "application/octet-stream"));
            //return HttpUnauthorized();          
        }
    }
}


//////below code locate physcial file on server 

////string fileName = "Valami.json";
////var filePath = Path.Combine(_contentDirectoryPath, fileName);

////// var localFilePath = HttpContext.Current.Server.MapPath("~/timetable.zip");
////HttpResponseMessage response = null;
////if (!System.IO.File.Exists(filePath))
////{
////    //if file not found than return response as resource not present 
////   // response = Request.CreateResponse(HttpStatusCode.Gone);
////}
////else
////{
////    //if file present than read file 
////    var fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
////    //compose response and include file as content in it
////    response = new HttpResponseMessage
////    {
////        StatusCode = HttpStatusCode.OK,
////        Content = new StreamContent(fStream)
////    };
////    //set content header of reponse as file attached in reponse
////    response.Content.Headers.ContentDisposition =
////                    new ContentDispositionHeaderValue("attachment")
////                    {
////                        FileName = Path.GetFileName(fStream.Name)
////                    };
////    //set the content header content type as application/octet-stream as it returning file as reponse 
////    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
////}
////return response;