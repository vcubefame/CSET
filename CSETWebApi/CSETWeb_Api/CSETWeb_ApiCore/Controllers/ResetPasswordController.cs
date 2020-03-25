//////////////////////////////// 
// 
//   Copyright 2020 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWeb_Api.Helpers;
using DataLayerCore.Model;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CSETWeb_Api.Models;
using CSETWeb_Api.BusinessManagers;
using CSETWeb_Api.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSETWeb_Api.Controllers
{
    public class ResetPasswordController : ControllerBase
    {
        private Regex emailvalidator = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
        private CSET_Context db = new CSET_Context();

        [HttpGet]
        [Route("api/ResetPassword/ResetPasswordStatus")]
        [Authorize]
        public IActionResult GetResetPasswordStatus()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Model State");
                }
                int userid = Auth.GetUserId();
                var rval = db.USERS.Where(x => x.UserId == userid).FirstOrDefault();
                if (rval != null)
                {
                    var resetRequired = rval.PasswordResetRequired;
                    return Ok(resetRequired);
                }

                return StatusCode(500,"Unknown error");

            }
            catch (CSETApplicationException ce)
            {
                return StatusCode(409, ce.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        /// <summary>
        /// performs an actual password change
        /// </summary>
        /// <param name="changePass"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ResetPassword/ChangePassword")]
        [Authorize]
        public async Task<IActionResult> PostChangePassword([FromBody] ChangePassword changePass)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Model State");
                }
                if (!emailvalidator.IsMatch(changePass.PrimaryEmail.Trim()))
                {
                    return BadRequest("Invalid PrimaryEmail");
                }

                Login login = new Login()
                {
                    Email = changePass.PrimaryEmail,
                    Password = changePass.CurrentPassword
                };
                LoginResponse resp = UserAuthentication.Authenticate(login);
                if (resp == null)
                {
                    return StatusCode(409,"Current password is invalid. Try again or request a new temporary password.");
                }

                UserAccountSecurityManager resetter = new UserAccountSecurityManager();

                bool rval = await resetter.ChangePassword(changePass);
                if (rval)
                {
                    resp.ResetRequired = false;
                    await db.SaveChangesAsync();
                    return Ok("Created Successfully");
                }
                else
                    return StatusCode(500,"Unknown error");

            }
            catch (CSETApplicationException ce)
            {
                return StatusCode(409, ce.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPost]
        [Route("api/ResetPassword/RegisterUser")]
        public async Task<IActionResult> PostRegisterUser([FromBody] CreateUser user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid Model State");
                }
                if (String.IsNullOrWhiteSpace(user.PrimaryEmail))
                    return BadRequest("Invalid PrimaryEmail");

                if (!emailvalidator.IsMatch(user.PrimaryEmail))
                {
                    return BadRequest("Invalid PrimaryEmail");
                }
                if (!emailvalidator.IsMatch(user.ConfirmEmail.Trim()))
                {
                    return BadRequest("Invalid PrimaryEmail");
                }
                if (user.PrimaryEmail != user.ConfirmEmail)
                    return BadRequest("Invalid PrimaryEmail");

                if (new UserManager().GetUserDetail(user.PrimaryEmail) != null)
                {
                    return BadRequest("An account already exists for that email address");
                }

                UserAccountSecurityManager resetter = new UserAccountSecurityManager();
                bool rval = resetter.CreateUserSendEmail(user);
                if (rval)
                    return Ok("Created Successfully");
                else
                    return StatusCode(500,"Unknown error");
            }
            catch (CSETApplicationException ce)
            {
                return StatusCode(409,ce.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPost]
        [Route("api/ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] SecurityQuestionAnswer answer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!emailvalidator.IsMatch(answer.PrimaryEmail.Trim()))
                {
                    return BadRequest();
                }

                if (IsSecurityAnswerCorrect(answer))
                {
                    UserAccountSecurityManager resetter = new UserAccountSecurityManager();
                    bool rval = await resetter.ResetPassword(answer.PrimaryEmail, "Password Reset", answer.AppCode);
                    if (rval)
                        return Ok();
                    else
                        return StatusCode(500);
                }

                // return Unauthorized();
                // returning a 401 (Unauthorized) gets caught by the JWT interceptor and dumps the user out, which we don't want.
                return Conflict();

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [Route("api/ResetPassword/PotentialQuestions")]
        [HttpGet]
        public IActionResult GetPotentialQuestions()
        {
            try
            {
                UserAccountSecurityManager resetter = new UserAccountSecurityManager();
                return Ok(resetter.GetSecurityQuestionList());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Route("api/ResetPassword/SecurityQuestions")]
        [HttpGet]
        public async Task<IActionResult> GetSecurityQuestions([FromQuery] string email, [FromQuery] string appCode)
        {
            try
            {
                if (db.USERS.Where(x => String.Equals(x.PrimaryEmail, email)).FirstOrDefault() == null)
                    return Conflict();

                List<SecurityQuestions> questions = await (from b in db.USER_SECURITY_QUESTIONS
                                                           join c in db.USERS on b.UserId equals c.UserId
                                                           where c.PrimaryEmail.Equals(email, StringComparison.CurrentCultureIgnoreCase)
                                                           select new SecurityQuestions()
                                                           {
                                                               SecurityQuestion1 = b.SecurityQuestion1,
                                                               SecurityQuestion2 = b.SecurityQuestion2
                                                           }).ToListAsync<SecurityQuestions>();
                //note that you don't have to provide a security question
                //it will just reset if you don't 
                if (questions.Count == 0 
                    || (questions[0].SecurityQuestion1 == null && questions[0].SecurityQuestion2 == null))
                {
                    UserAccountSecurityManager resetter = new UserAccountSecurityManager();
                    bool rval = await resetter.ResetPassword(email, "Password Reset", appCode);
                    return Ok(new List<SecurityQuestions>());
                }


                return Ok(questions);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        /// <summary>
        /// Checks the user-supplied question and answer against the stored answer.
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        private bool IsSecurityAnswerCorrect(SecurityQuestionAnswer answer)
        {
            var questions = from b in db.USER_SECURITY_QUESTIONS
                            join c in db.USERS on b.UserId equals c.UserId
                            where c.PrimaryEmail.Equals(answer.PrimaryEmail, StringComparison.CurrentCultureIgnoreCase)
                            && (
                                (b.SecurityQuestion1 == answer.QuestionText
                                    && b.SecurityAnswer1.Equals(answer.AnswerText, StringComparison.InvariantCultureIgnoreCase))
                                || (b.SecurityQuestion2 == answer.QuestionText
                                    && b.SecurityAnswer2.Equals(answer.AnswerText, StringComparison.InvariantCultureIgnoreCase))
                                )
                            select b;

            if ((questions != null) && questions.FirstOrDefault() != null)
                return true;
            return false;
        }
    }
}




