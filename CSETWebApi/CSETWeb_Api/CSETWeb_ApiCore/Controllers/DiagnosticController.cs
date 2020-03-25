//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System;
using Microsoft.AspNetCore.Mvc;

namespace CSETWeb_Api.Controllers
{
    /// <summary>
    /// Diagnostic functionality.
    /// </summary>
    public class DiagnosticController : ControllerBase
    {
        /// <summary>
        /// Tests connectivity to the SMTP server and sends 
        /// a test email to the designated recipient.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/diagnostic/email")]
        public string TestEmailServer([FromQuery]string recip)
        {
            try
            {
                BusinessLogic.NotificationManager nm = new BusinessLogic.NotificationManager();
                nm.SendTestEmail(recip);
            }
            catch (Exception Exc)
            {
                return Exc.Message;
            }

            return "Test email sent successfully";
        }
    }
}


