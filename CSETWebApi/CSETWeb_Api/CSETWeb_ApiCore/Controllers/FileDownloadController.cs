//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWeb_Api.Helpers;
using DataAccess;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    public class FileDownloadController : ControllerBase
    {
        private readonly FileRepository fileRepo = new FileRepository();

        [HttpGet]
        [Route("api/files/download/{id}")]
        public Task<HttpResponseMessage> Download(int id, string token)
        {
            var assessmentId = Auth.AssessmentForUser(token);
            var file = fileRepo.GetFileDescription(id);
            var stream = new MemoryStream(file.Data);
            
            var result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = file.Name };
            return Task.FromResult(result);
        }
    }
}
