//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;
using CSETWeb_Api.Helpers;
using CSETWeb_Api.BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CSETAuthorize]
    public class AggregationController : ControllerBase
    {
        /// <summary>
        /// Returns a list of aggregations that the current user is allowed to see.
        /// The user must be authorized to view all assessments involved in the aggregation.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/aggregation/getaggregations")]
        public IActionResult GetAggregations([FromQuery] string mode)
        {
            // Get the current userid to set as the Assessment creator and first attached user
            int currentUserId = Auth.GetUserId();

            var manager = new BusinessLogic.AggregationManager();
            return Ok(manager.GetAggregations(mode, currentUserId));
        }


        [HttpPost]
        [Route("api/aggregation/create")]
        public IActionResult CreateAggregation([FromQuery] string mode)
        {
            var manager = new BusinessLogic.AggregationManager();
            return Ok(manager.CreateAggregation(mode));
        }


        [HttpPost]
        [Route("api/aggregation/get")]
        public IActionResult GetAggregation()
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return null;
            }

            var manager = new BusinessLogic.AggregationManager();
            return Ok(manager.GetAggregation((int)aggregationID));
        }


        [HttpPost]
        [Route("api/aggregation/update")]
        public IActionResult UpdateAggregation([FromBody] Aggregation aggregation)
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return Ok();
            }

            var manager = new BusinessLogic.AggregationManager();
            manager.SaveAggregationInformation(aggregation.AggregationId, aggregation);
            return Ok();
        }


        [HttpPost]
        [Route("api/aggregation/delete")]
        public IActionResult DeleteAggregation([FromQuery] int aggregationId)
        {
            var manager = new BusinessLogic.AggregationManager();
            manager.DeleteAggregation(aggregationId);
            return Ok();
        }


        [HttpPost]
        [Route("api/aggregation/getassessments")]
        public IActionResult GetAssessmentsForAggregation()
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return null;
            }

            var manager = new BusinessLogic.AggregationManager();
            return Ok(manager.GetAssessmentsForAggregation((int)aggregationID));
        }


        [HttpPost]
        [Route("api/aggregation/saveassessmentselection")]
        public IActionResult SaveAssessmentSelection([FromBody] AssessmentSelection request)
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return Ok();
            }

            var aggreg = new BusinessLogic.AggregationManager();
            return Ok(aggreg.SaveAssessmentSelection((int)aggregationID, request.AssessmentId, request.Selected));
        }


        [HttpPost]
        [Route("api/aggregation/saveassessmentalias")]
        public IActionResult SaveAssessmentAlias([FromBody] AssessmentSelection request)
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return Ok();
            }

            var aggreg = new BusinessLogic.AggregationManager();
            aggreg.SaveAssessmentAlias((int)aggregationID, request.AssessmentId, request.Alias);
            return Ok();
        }


        [HttpPost]
        [Route("api/aggregation/getmissedquestions")]
        public IActionResult GetCommonlyMissedQuestions()
        {
            TokenManager tm = new TokenManager();
            var aggregationID = tm.PayloadInt("aggreg");
            if (aggregationID == null)
            {
                return Ok(new List<MissedQuestion>());
            }

            var manager = new BusinessLogic.AggregationManager();
            return Ok(manager.GetCommonlyMissedQuestions((int)aggregationID));
        }



        //////////////////////////////////////////
        /// Merge
        //////////////////////////////////////////

        [HttpPost]
        [Route("api/aggregation/getanswers")]
        public IActionResult GetAnswers()
        {
            var aggreg = new BusinessLogic.AggregationManager();
            return Ok(aggreg.GetAnswers(new List<int>() { 4, 5 }));
        }


        /// <summary>
        /// Sets a single answer text into the COMBINED_ANSWER table.
        /// </summary>
        [HttpPost]
        [Route("api/aggregation/setmergeanswer")]
        public IActionResult SetMergeAnswer([FromQuery] int answerId, [FromQuery] string answerText)
        {
            var aggreg = new BusinessLogic.AggregationManager();
            aggreg.SetMergeAnswer(answerId, answerText);
            return Ok();
        }
    }
}
