using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AffiliateController : ControllerBase
    {
        private readonly IAffiliateService _affiliateService;

        public AffiliateController(IAffiliateService affiliateService)
        {
            _affiliateService = affiliateService;
        }

        [HttpPost("registration")]
        [ProducesResponseType(typeof(RegistrationResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RegistrationResponse>> Registration(
            [FromBody] RegistrationRequest request)
        {
            var affiliateId = string.Empty;
            var apiKey = string.Empty;

            if (Request.Headers.TryGetValue("affiliate-id", out var value1))
            {
                affiliateId = value1;
            }
            if (Request.Headers.TryGetValue("api-key", out var value2))
            {
                apiKey = value2;
            }

            if (string.IsNullOrWhiteSpace(affiliateId) ||
                string.IsNullOrWhiteSpace(apiKey))
            {
                return BadRequest(new RegistrationResponse() {Success = false, ErrorMessage = "Master affiliate headers not found."});
            }
            
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Email) || 
                string.IsNullOrWhiteSpace(request.LandingUrl))
            {
                return BadRequest(new RegistrationResponse() {Success = false, ErrorMessage = "Cannot create affiliate with empty fields."});
            }
            try
            {
                // TODO: call CreateSybAsync
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
                //ar cachedEntities = await _registrationDataWriter.GetAsync();
                //ar registrationEntity = cachedEntities.FirstOrDefault(e => e.Registration.Token == token);
            
                //f (registrationEntity != null)
                //
                //   if (registrationEntity.Registration.RequestDate.Date != DateTime.UtcNow.Date)
                //   {
                //       _logger.LogInformation($"Deny confirmation for : {registrationEntity.Registration.Email}.");
                //       return false;
                //   }
                
                // 
                
                //   _logger.LogInformation($"Performed confirmation for : {registrationEntity.Registration.Email}.");
                //   await _registrationDataWriter.DeleteAsync(registrationEntity.PartitionKey, registrationEntity.RowKey);
                //   return true;
                //
            
                //logger.LogInformation($"Cannot find token :{token}.");
                
                //f (confirmation)
                //   return RedirectPermanent(Program.Settings.ConfirmationRedirectUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new ConfirmationResponse() {Success = false, ErrorMessage = ex.Message});
            }
            return Ok(new ConfirmationResponse(){Success = false, ErrorMessage = "Cannot find token. Please try again."});
        }
    }
}