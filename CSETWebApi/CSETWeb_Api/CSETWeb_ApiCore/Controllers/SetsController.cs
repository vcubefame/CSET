//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using BusinessLogic.Models;
using CSETWeb_Api.BusinessLogic.Helpers;
using CSETWeb_Api.Helpers;
using DataLayerCore.Model;
using Hangfire;
using Hangfire.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    [Authorize]
    public class SetsController : ControllerBase
    {
        [HttpGet]
        [Route("api/sets")]
        public IActionResult GetAllSets()
        {
            using (var db = new CSET_Context())
            {
                var sets = db.SETS.Where(s => s.Is_Displayed ?? true)
                    .Select(s => new { Name = s.Full_Name, SetName = s.Set_Name })
                    .OrderBy(s => s.Name)
                    .ToArray();
                return Ok(sets);
            }
        }

        [Route("api/sets/import/status/{id}")]
        [HttpGet]
        public IActionResult GetImportStatus([FromQuery] string id)
        {
            var state = Hangfire.States.AwaitingState.StateName;
            try
            {
                var job = JobStorage.Current.GetMonitoringApi().JobDetails(id);
                if (job != null)
                {
                    var history = job.History.OrderByDescending(s => s.CreatedAt);
                    state = (
                                history.FirstOrDefault(s => s.StateName == Hangfire.States.SucceededState.StateName) ??
                                history.FirstOrDefault(s => s.StateName == Hangfire.States.FailedState.StateName) ??
                                history.FirstOrDefault(s => s.StateName == Hangfire.States.ProcessingState.StateName)
                            )?.StateName ?? state;
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(new { state });
        }

        /// <summary>
        /// Import new standards into CSET
        /// </summary>
        [HttpPost]
        [Route("api/sets/import")]
        public IActionResult Import([FromBody] ExternalStandard externalStandard)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = BackgroundJob.Enqueue(() => HangfireExecutor.SaveImport(externalStandard, null));
                    return Ok(new { id });
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("api/sets/export/{setName}")]
        public IActionResult Export([FromQuery] string setName)
        {
            var response = default(HttpResponseMessage);
            using (var db = new CSET_Context())
            {
                var set = db.SETS
                    .Include(s => s.Set_Category_)
                    .Include(s => s.REQUIREMENT_SETS)
                    .ThenInclude(r => r.Requirement_)
                    .ThenInclude(rf => rf.REQUIREMENT_REFERENCES)
                    .ThenInclude(gf => gf.Gen_File_)
                    .Where(s => (s.Is_Displayed ?? false) && s.Set_Name == setName).FirstOrDefault();

                if (set == null)
                {
                    return StatusCode(404, $"A Set named '{setName}' was not found.");
                }
                return Ok(set.ToExternalStandard());
            }
        }
    }
}
