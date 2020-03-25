//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CSETWeb_Api.Controllers
{
    public class AngularConfigController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AngularConfigController(IFileProvider fileProvider, IWebHostEnvironment webHostEnvironment)
        {
            _fileProvider = fileProvider;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// NOTE THIS APOLOGY
        /// this call returns the config.json file
        /// but modifies the port to be the current port 
        /// the application is running on.
        /// (IE the file may be different from what is returned)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assets/config")]
        [Route("Reports/api/assets/config")]
        public IActionResult GetConfigURLRewrite(HttpRequestMessage requestMessage)
        {
            return Ok(processConfig(requestMessage.RequestUri));
        }

        
        private IActionResult processConfig(Uri newBase)
        {
            var rootPath = _webHostEnvironment.WebRootPath;
            if (System.IO.File.Exists(rootPath + "/assets/config.json"))
            {
                string contents = System.IO.File.ReadAllText(rootPath + "/assets/config.json");
                var jObject = JObject.Parse(contents);
                if (jObject["override"] != null)
                    if ((jObject["override"]).ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        return Ok(jObject);

                // get the base appURL 
                // then change it to include the new port.
                string findString = jObject["appUrl"].ToString();
                string replaceString = newBase.GetLeftPart(UriPartial.Authority) + "/";

                if (findString.SequenceEqual(replaceString))
                    return Ok(jObject);

                jObject["appUrl"] = newUri(newBase, (jObject["appUrl"]).ToString());
                jObject["apiUrl"] = newUri(newBase, (jObject["apiUrl"]).ToString());
                jObject["docUrl"] = newUri(newBase, (jObject["docUrl"]).ToString());
                String reportsUrl = (jObject["reportsUrl"]).ToString();
                if (System.IO.File.Exists(rootPath + "/reports/index.html"))
                {
                    reportsUrl += reportsUrl.EndsWith("reports/", StringComparison.CurrentCultureIgnoreCase) ? "" : "reports/";
                }
                jObject["reportsUrl"] = newUri(newBase, reportsUrl);                
                return Ok(jObject);
            }
            return NotFound("assets/config.json file not found");
        }


        private Uri newUri(Uri newBase, string oldUri)
        {
            //set the hostname and port to the same as the new base return the new uri
            UriBuilder tmp = new UriBuilder(oldUri);
            tmp.Host = newBase.Host;
            if ((newBase.Port == 80) || (newBase.Port == 443))
                tmp.Port = -1;
            else
                tmp.Port = newBase.Port;
            tmp.Scheme = newBase.Scheme;

            return tmp.Uri; 
        }
    }
}


