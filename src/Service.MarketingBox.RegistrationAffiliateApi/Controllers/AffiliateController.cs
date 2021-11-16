using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;
using Service.MarketingBox.RegistrationAffiliateApi.Engines;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AffiliateController : ControllerBase
    {
        private readonly AffiliateEngine _affiliateEngine;

        public AffiliateController(AffiliateEngine affiliateEngine)
        {
            _affiliateEngine = affiliateEngine;
        }

        [HttpPost("registration")]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegistrationResponse>> Registration(
            [FromBody] RegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new RegistrationResponse() {Success = false, ErrorMessage = "Cannot process empty strings."});
            }
            try
            {
                await _affiliateEngine.Registration(request.Username, request.Password, request.Email);
            }
            catch (Exception ex)
            {
                return BadRequest(new RegistrationResponse() {Success = false, ErrorMessage = ex.Message});
            }
            return Ok(new RegistrationResponse(){Success = true});
        }
        
        [HttpGet("confirmation/{token}")]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ConfirmationResponse>> Confirmation(
            [FromRoute, Required] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new ConfirmationResponse() {Success = false, ErrorMessage = "Cannot process empty token."});
            }
            try
            {
                var confirmation = await _affiliateEngine.Confirmation(token);
                
                if (confirmation)
                    return RedirectPermanent(Program.Settings.ConfirmationRedirectUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new ConfirmationResponse() {Success = false, ErrorMessage = ex.Message});
            }
            return Ok(new ConfirmationResponse(){Success = false, ErrorMessage = "Cannot find token. Please try again."});
        }
    }
}