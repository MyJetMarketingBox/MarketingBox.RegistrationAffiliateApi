using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.MarketingBox.RegistrationAffiliateApi.Engines;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/affiliates")]
    public class AffiliateController : ControllerBase
    {
        private readonly AffiliateEngine _affiliateEngine;

        public AffiliateController(AffiliateEngine affiliateEngine)
        {
            _affiliateEngine = affiliateEngine;
        }

        [HttpPost]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegistrationResponse>> Registration(
            [FromBody] RegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Cannot process empty strings.");
            }

            try
            {
            }
            catch (Exception ex)
            {
                
            }
            return Ok(new RegistrationResponse(){Success = true, ErrorMessage = request.Email});
        }
    }

    public class RegistrationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class RegistrationResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}