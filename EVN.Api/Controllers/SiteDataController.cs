using EVN.Api.Jwt;
using EVN.Core.CMIS;
using FX.Core;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EVN.Api.Controllers
{
    [RoutePrefix("api/sitedata")]
    public class SiteDataController : ApiController
    {
        [JwtAuthentication]
        [HttpGet]
        [Route("{*path}")]
        public HttpResponseMessage Get(string path)
        {
            try
            {
                //Create HTTP Response.
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                if (string.IsNullOrEmpty(path))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("NO PATH FILE!");
                    throw new HttpResponseException(response);
                }
                string physicalSiteDataDirectory = ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"];
                var filePath = Path.Combine(physicalSiteDataDirectory, path);
                var fileName = Path.GetFileName(filePath);

                //Check whether File exists.
                if (!File.Exists(filePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = string.Format("File not found.", fileName);
                    throw new HttpResponseException(response);
                }

                //Read the File into a Byte Array.
                byte[] bytes = File.ReadAllBytes(filePath);

                var stream = new MemoryStream(bytes);
                // processing the stream.

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "CertificationCard.pdf"
                    };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        //[JwtAuthentication]
        //[HttpGet]
        //[Route("GetByteArray")]
        //public IHttpActionResult GetByteArray(string path)
        //{
        //    string type = "";
        //    if (path.Contains("xlsx"))
        //    {
        //        type = "xlsx";
        //    }if (path.Contains("pdf"))
        //    {
        //        type = "pdf";
        //    }if (path.Contains("doc"))
        //    {
        //        type = "doc";
        //    }
        //    //string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}{path.Replace($@"///", $@"\")}";
        //    string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{path}";

        //    //Check whether File exists.
        //    if (!System.IO.File.Exists(fullPath))
        //    {
        //        return NotFound();
        //    }

        //    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        int length = Convert.ToInt32(fs.Length);
        //        byte[] buff = new byte[length];
        //        fs.Read(buff, 0, length);

        //        return Ok(new viewFile { BaseType = buff , Type = type });
        //    }
        //}

        //[JwtAuthentication]
        [HttpGet]
        [Route("GetByteArray")]
        public IHttpActionResult GetByteArray(string path)
        {
            string type = "";
            if (path.Contains("xlsx"))
            {
                type = "xlsx";
            }
            if (path.Contains("pdf"))
            {
                type = "pdf";
            }
            if (path.Contains("doc"))
            {
                type = "doc";
            }
            string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}{path.Replace($@"///", $@"\")}";
            fullPath = fullPath.Replace("/", "\\");
            //string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{path}";

            //Check whether File exists.
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int length = Convert.ToInt32(fs.Length);
                byte[] buff = new byte[length];
                fs.Read(buff, 0, length);

                return Ok(new viewFile { BaseType = buff, Type = type });
            }

    }

        //[JwtAuthentication]
        [HttpGet]
        [Route("GetByteArray1")]
        public IHttpActionResult GetByteArray1(string path)
        {
            string fullPath = $"{ConfigurationManager.AppSettings["PhysicalSiteDataDirectory"]}/{path}";

            //Check whether File exists.
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int length = Convert.ToInt32(fs.Length);
                byte[] buff = new byte[length];
                fs.Read(buff, 0, length);
                return Ok(buff);
            }
        }

        public class viewFile
        {
            public string Type { get; set; }
            public byte[] BaseType { get; set; }
        }
    }
}
