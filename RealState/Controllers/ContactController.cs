using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealState.BAL.DTO;
using RealState.BAL.Helpers;
using RealState.BAL.ILogic;
using RealState.BAL.Logic;
using RealState.Email;
using RealState.Repository.Repository;
using System.Text;

namespace RealState.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This matters
    public class ContactController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        protected IContactLogic _ContactLogicBAL { get; private set; }
        private readonly IOptions<AppSettingsDTO> _appSettings;
        private readonly ILogger<ContactController> _logger; // Logger instance

        #region CTOR's

        public ContactController(IContactLogic ContactLogicBAL, IOptions<AppSettingsDTO> appSettings, ILogger<ContactController> logger)
        {
            _ContactLogicBAL = ContactLogicBAL;
            _appSettings = appSettings;
            _logger = logger; // Initialize logger
        }

        #endregion

        [HttpGet]
        [Route("GetAllPaymentFrequrency")]
        public IActionResult GetAllPaymentFrequrency()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responseObj = _ContactLogicBAL.GetPaymentFrequrency("");
                if (responseObj == null)
                    return NotFound();

                return Ok(responseObj);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllPaymentFrequrency.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetcanadacitiesList")]
        public IActionResult GetcanadacitiesList()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responseObj = _ContactLogicBAL.Getcanadacities("");
                if (responseObj == null)
                    return NotFound();

                return Ok(responseObj);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "An error occurred in GetcanadacitiesList.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetAllAmortizationfrequency")]
        public IActionResult GetAllAmortizationfrequency()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responseObj = _ContactLogicBAL.GetAmortizationfrequency("");
                if (responseObj == null)
                    return NotFound();

                return Ok(responseObj);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllAmortizationfrequency.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("AddContacts")]
        public async Task<IActionResult> AddContacts([FromBody] ContactDTO Obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var responseObj = await _ContactLogicBAL.AddContact(Obj);
                if (string.IsNullOrEmpty(responseObj))
                {
                    return NotFound();
                }
                return Ok(responseObj);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, "An error occurred in AddContacts.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
