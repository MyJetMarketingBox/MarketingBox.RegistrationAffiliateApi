using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.MarketingBox.Email.Service.Domain.Models;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AffiliateController : ControllerBase
    {
        private readonly IAffiliateService _affiliateService;
        private readonly ILogger<AffiliateController> _logger;
        private readonly IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> _dataWriter;

        public AffiliateController(IAffiliateService affiliateService, 
            ILogger<AffiliateController> logger, 
            IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> dataWriter)
        {
            _affiliateService = affiliateService;
            _logger = logger;
            _dataWriter = dataWriter;
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
                var response = await _affiliateService.CreateSubAsync(new CreateSubRequest()
                {
                    Username = request.Username,
                    Password = request.Password,
                    Email = request.Email,
                    LandingUrl = request.LandingUrl,
                    MasterAffiliateId = affiliateId,
                    MasterAffiliateApiKey = apiKey,
                    Sub = request.Sub
                });

                _logger.LogInformation("Get response from _affiliateService.CreateSubAsync: {responseJson}", 
                    JsonConvert.SerializeObject(response));
                
                if (response.Affiliate != null &&
                    response.Error == null)
                {
                    return Ok(new RegistrationResponse(){Success = true});
                }

                return Problem(response.Error?.Message ?? "Cannot get error message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new RegistrationResponse() {Success = false, ErrorMessage = ex.Message});
            }
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
                var cachedEntities = await _dataWriter.GetAsync();
                var registrationEntity = cachedEntities.FirstOrDefault(e => e.Entity.Token == token);

                if (registrationEntity == null || 
                    registrationEntity.Expires < DateTime.UtcNow)
                {
                    var error = $"Deny confirmation with token : {token}.";
                    _logger.LogInformation(error);
                    return BadRequest(new ConfirmationResponse() {Success = false, ErrorMessage = error});
                }

                _logger.LogInformation($"Performed confirmation for : {registrationEntity.Entity.AffiliateId}.");
                
                // TODO: updateAsync 
                
                await _dataWriter.DeleteAsync(registrationEntity.PartitionKey, registrationEntity.RowKey);
                
                return RedirectPermanent(Program.Settings.ConfirmationRedirectUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new ConfirmationResponse() {Success = false, ErrorMessage = ex.Message});
            }
        }
    }
}