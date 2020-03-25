using CSETWeb_Api.BusinessLogic.BusinessManagers;
using CSETWeb_Api.BusinessLogic.BusinessManagers.Analysis;
using CSETWeb_Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CSETWeb_Api.Controllers
{
    [Authorize]
    public class MaturityController : ControllerBase
    {
        /// <summary>
        /// Get maturity calculations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getMaturityResults")]
        public IActionResult GetMaturityResults()
        {
            int assessmentId = Auth.AssessmentForUser();
            MaturityManager manager = new MaturityManager();
            var maturity = manager.GetMaturityAnswers(assessmentId);

            return Ok(maturity);
        }

        /// <summary>
        /// Get maturity range based on IRP rating
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getMaturityRange")]
        public IActionResult GetMaturityRange()
        {
            int assessmentId = Auth.AssessmentForUser();
            MaturityManager manager = new MaturityManager();
            var maturityRange = manager.GetMaturityRange(assessmentId);
            return Ok(maturityRange);
        }

        /// <summary>
        /// Get IRP total for maturity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getOverallIrpForMaturity")]
        public IActionResult GetOverallIrp()
        {
            int assessmentId = Auth.AssessmentForUser();
            return Ok(new ACETDashboardManager().GetOverallIrp(assessmentId));
        }

        /// <summary>
        /// Get target band for maturity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getTargetBand")]
        public IActionResult GetTargetBand()
        {
            int assessmentId = Auth.AssessmentForUser();
            return Ok(new MaturityManager().GetTargetBandOnly(assessmentId));
        }

        /// <summary>
        /// Set target band for maturity rating
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/setTargetBand")]
        public IActionResult SetTargetBand([FromBody]bool value)
        {
            int assessmentId = Auth.AssessmentForUser();
            new MaturityManager().SetTargetBandOnly(assessmentId, value);
            return Ok();
        }
    }
}
