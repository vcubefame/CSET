using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace CSETWeb_Api.Controllers
{
    //[CSETAuthorize]
    public class GuidController : ControllerBase
    {
        
        [Route("api/guid/requestblock")]
        [HttpGet]
        public List<Guid> GetABlockOfGuids([FromQuery] int number=100)
        {
            List<Guid> guids = new List<Guid>();
            for(int i=0; i<number; i++)
            {
                guids.Add(Guid.NewGuid());
            }
            return guids;
        }
    }
}