using CSETWeb_Api.BusinessLogic.BusinessManagers.AdminTab;
using CSETWeb_Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{

    [Authorize]
    public class AdminTabController : ControllerBase
    {
        [HttpPost,HttpGet]
        [Route("api/admintab/Data")]
        public IActionResult GetList()
        {
            int assessmentId = Auth.AssessmentForUser();                                    
            AdminTabManager manager = new AdminTabManager();
            return Ok(manager.GetTabData(assessmentId));            
        }

        [HttpPost]
        [Route("api/admintab/save")]
        public IActionResult SaveData([FromBody]AdminSaveData save)
        {
            int assessmentId = Auth.AssessmentForUser();                       
            AdminTabManager manager = new AdminTabManager();
            return Ok(manager.SaveData(assessmentId, save));
        }


        [HttpPost]
        [Route("api/admintab/saveattribute")]
        public void SaveDataAttribute([FromBody] AttributePair attribute)
        {
            int assessmentId = Auth.AssessmentForUser();            
            AdminTabManager manager = new AdminTabManager();
            manager.SaveDataAttribute(assessmentId, attribute);
        }
    }

   
}
