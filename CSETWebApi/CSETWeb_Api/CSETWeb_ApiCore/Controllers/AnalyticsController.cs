using System.Collections.Generic;
using System.Linq;
using CSETWeb_Api.BusinessLogic.Models;
using CSETWeb_Api.BusinessManagers;
using CSETWeb_Api.Helpers;
using CSETWeb_Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        /// <summary>
        /// Get analytic information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/analytics/getAnalytics")]
        public IActionResult GetAnalytics()
        {
            return Ok(new Analytics
            {
                Demographics = GetDemographics(),
                QuestionAnswers = GetQuestionsAnswers()
            });
        }

        /// <summary>
        /// Returns an instance of Demographics for Anonymous export 
        /// </summary>        
        /// <returns></returns>
        private AnalyticsDemographic GetDemographics()
        {
            int assessmentId = Auth.AssessmentForUser();
            AssessmentManager assessmentManager = new AssessmentManager();
            return assessmentManager.GetAnonymousDemographics(assessmentId);
        }

        /// <summary>
        /// Returns questions/answers for current selected assessment
        /// </summary>
        /// <returns></returns>
        private List<AnalyticsQuestionAnswer> GetQuestionsAnswers()
        {
            int assessmentId = Auth.AssessmentForUser();
            var questionsController = new QuestionsController();
            string applicationMode = questionsController.GetApplicationMode(assessmentId);
            QuestionsManager qm = new QuestionsManager(assessmentId);

            if (applicationMode.ToLower().StartsWith("questions"))
            {
                QuestionResponse resp = qm.GetQuestionList("*");
                return qm.GetAnalyticQuestionAnswers(resp).OrderBy(x=>x.QuestionId).ToList();
            }
            else
            {
                RequirementsManager rm = new RequirementsManager(assessmentId);
                QuestionResponse resp = rm.GetRequirementsList();
                return qm.GetAnalyticQuestionAnswers(resp).OrderBy(x=>x.QuestionId).ToList();
            }
        }

        
    }
}
