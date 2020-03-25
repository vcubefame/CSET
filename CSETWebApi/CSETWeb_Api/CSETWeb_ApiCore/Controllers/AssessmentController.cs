//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.Collections.Generic;
using CSETWeb_Api.Helpers;
using CSETWeb_Api.Models;
using CSETWeb_Api.BusinessManagers;
using DataLayerCore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    /// <summary>
    /// Manages assessments.
    /// </summary>
    [Authorize]
    public class AssessmentController : ControllerBase
    {
        /// <summary>
        /// Creates a new Assessment with the current user as the first contact
        /// in an admin role.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/createassessment")]
        public IActionResult CreateAssessment()
        {   
            // Get the current userid to set as the Assessment creator and first attached user
            int currentUserId = Auth.GetUserId();

            AssessmentManager man = new AssessmentManager();
            return Ok(man.CreateNewAssessment(currentUserId));            
        }


        /// <summary>
        /// Returns an array of Assessments connected to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assessmentsforuser")]
        public IActionResult GetMyAssessments()
        {
            // get all Assessments that the current user is associated with
            AssessmentManager assessmentManager = new AssessmentManager();
            return Ok(assessmentManager.GetAssessmentsForUser(TransactionSecurity.CurrentUserId));
        }


        /// <summary>
        /// Returns the AssessmentDetail for current Assessment defined in the security token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assessmentdetail")]
        public AssessmentDetail Get()
        {
            // Get the AssessmentId from the token
            int assessmentId = Auth.AssessmentForUser();

            AssessmentManager assessmentManager = new AssessmentManager();
            return assessmentManager.GetAssessmentDetail(assessmentId);
        }


        /// <summary>
        /// Persists the posted AssessmentDetail.
        /// </summary>
        /// <param name="assessmentDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/assessmentdetail")]
        public IActionResult Post([FromBody]AssessmentDetail assessmentDetail)
        {
            // validate the assessment for the user
            int assessmentId = Auth.AssessmentForUser();
            if (assessmentId != assessmentDetail.Id)
            {
                return Unauthorized("Not currently authorized to update the Assessment");
            }

            AssessmentManager assessmentManager = new AssessmentManager();
            return Ok(assessmentManager.SaveAssessmentDetail(assessmentId, assessmentDetail));
        }


        /// <summary>
        /// Returns a collection of all documents attached to any question in the Assessment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assessmentdocuments")]
        public IActionResult GetDocumentsForAssessment()
        {
            int assessmentId = Auth.AssessmentForUser();

            DocumentManager dm = new DocumentManager(assessmentId);
            return Ok(dm.GetDocumentsForAssessment(assessmentId));
        }
    }
}


