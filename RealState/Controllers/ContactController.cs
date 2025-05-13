using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RealState.BAL.DTO;
using RealState.BAL.Helpers;
using RealState.BAL.ILogic;
using RealState.Email;
using RealState.Repository.Repository;
using System.Text;

namespace RealState.Controllers
{
    public class ContactController : Controller
    {
        protected IContactLogic _ContactLogicBAL { get; private set; }
        private readonly IOptions<AppSettingsDTO> _appSettings;
        #region CTOR's

        public ContactController(IContactLogic ContactLogicBAL, IOptions<AppSettingsDTO> appSettings
        )

        {
            _ContactLogicBAL = ContactLogicBAL;
            _appSettings = appSettings;

        }
        #endregion

        [HttpPost]
        [Route("AddContacts")]
        public async Task<IActionResult> AddContacts([FromBody] ContactDTO Obj)
        {
            try
            {
                Obj.CreatedDate = DateTime.Now;
                Obj.IsActive= true;
                Obj.EmailSend = false;
                Obj.ResponseStatus = false;
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var responseObj = await _ContactLogicBAL.AddContact(Obj);
                if (string.IsNullOrEmpty(responseObj))
                {
                    return NotFound();
                }

                var sb = new StringBuilder();
                sb.Append("Dear Support Team,<br>");
                sb.Append("<br>");
                sb.Append("A user has submitted a request through the Contact Us page regarding account access.<br>");
                sb.Append("Below are the user's details:<br>");
                sb.Append("<br>");
                sb.Append("Username (email): <b>" + Obj.Name + "</b><br>");
                sb.Append("Email address: <b>" + Obj.Email + "</b><br>");
                sb.Append("Phone number: <b>" + Obj.PhoneNumber + "</b><br>");
                sb.Append("Notes: <b>" + Obj.Message + "</b><br>");
                sb.Append("<br>");
                sb.Append("Please follow up with the user to assist them further.<br>");
                sb.Append("<br>");
                sb.Append("Regards,<br>");
                sb.Append("Pankaj Sadyal<br>");
                sb.Append("<br>");
                string emailBody = sb.ToString();
                bool emailResponse;
                EmailHelper emailHelper = new EmailHelper(_appSettings);
                emailResponse = emailHelper.SendEmailVerifyRequest("pankaj.sadiyal@spadezgroup.com", emailBody, "Contact Qurey");

                return Ok(responseObj);
            }
            catch (AppException ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
