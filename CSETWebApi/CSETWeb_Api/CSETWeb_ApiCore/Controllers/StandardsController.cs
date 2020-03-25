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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    /// <summary>
    /// Manages questions and their answers. 
    /// </summary>
    [Authorize]
    public class StandardsController : ControllerBase
    {
        /// <summary>
        /// Returns a list of all displayable cybersecurity standards.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/standards")]
        public StandardsResponse GetStandards()
        {
            int assessmentId = Auth.AssessmentForUser();
            return new StandardsManager().GetStandards(assessmentId);
        }


        /// <summary>
        /// Persists the current Standards selection in the database.
        /// </summary>
        [HttpPost]
        [Route("api/standard")]
        public QuestionRequirementCounts PersistSelectedStandards(List<string> selectedStandards)
        {
            int assessmentId = Auth.AssessmentForUser();
            return new StandardsManager().PersistSelectedStandards(assessmentId, selectedStandards);
        }

        /// <summary>
        /// Set default standard for basic assessment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/basicStandard")]
        public QuestionRequirementCounts PersistDefaultSelectedStandards()
        {
            int assessmentId = Auth.AssessmentForUser();
            return new StandardsManager().PersistDefaultSelectedStandard(assessmentId);
        }

        /// <summary>
        /// Persists the current Standards selection in the database.
        /// </summary>
        [HttpGet]
        [Route("api/standard/IsFramework")]
        public bool GetFrameworkSelected()
        {
            int assessmentId = Auth.AssessmentForUser();
            return new StandardsManager().GetFramework(assessmentId);
        }

        /// <summary>
        /// Persists the current Standards selection in the database.
        /// </summary>
        [HttpGet]
        [Route("api/standard/IsACET")]
        public bool GetACETSelected()
        {
            int assessmentId = Auth.AssessmentForUser();
            return new StandardsManager().GetACET(assessmentId);
        }
    }
}


