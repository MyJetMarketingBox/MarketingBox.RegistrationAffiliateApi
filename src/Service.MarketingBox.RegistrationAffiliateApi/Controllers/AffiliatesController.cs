using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using FluentValidation;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Newtonsoft.Json;
using Service.MarketingBox.Email.Service.Domain.Models;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;
using ValidationError = MarketingBox.Sdk.Common.Models.ValidationError;
using ValidationException = FluentValidation.ValidationException;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [ApiController]
    [Route("/api/affiliates")]
    public class AffiliatesController : ControllerBase
    {
        private readonly IAffiliateService _affiliateService;
        private readonly ILogger<AffiliatesController> _logger;
        private readonly IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> _dataWriter;

        public AffiliatesController(IAffiliateService affiliateService, 
            ILogger<AffiliatesController> logger, 
            IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> dataWriter)
        {
            _affiliateService = affiliateService;
            _logger = logger;
            _dataWriter = dataWriter;
        }

        [HttpPost("registration")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Registration(
            [Required, FromHeader(Name = "affiliate-id")] long affiliateId,
            [Required, FromHeader(Name = "api-key")] string apiKey,
            [FromBody] RegistrationRequest request,
            [FromServices] IValidator<RegistrationRequest> validator)
        {
            _logger.LogInformation($"AffiliatesController.Registration receive Haders: affiliateId - {affiliateId}, api-key - {apiKey}.");
            _logger.LogInformation($"AffiliatesController.Registration receive request: {JsonConvert.SerializeObject(request)}");
            
            try
            {
                await validator.ValidateAndThrowAsync(request);
                
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

                return this.ProcessResult(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new ApiException(new Error
                {
                    ErrorMessage = BadRequestException.DefaultErrorMessage,
                    ValidationErrors = ex.Errors.Select(
                        x=> new ValidationError
                        {
                            ErrorMessage = x.ErrorMessage,
                            ParameterName = x.PropertyName
                        }).ToList()
                });
            }
        }
        
        [HttpGet("confirmation/{token}")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Confirmation(
            [FromRoute, Required] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ApiException(new Error
                {
                    ErrorMessage = "Cannot process empty token."
                });
            }
            try
            {
                var cachedEntities = await _dataWriter.GetAsync();
                var registrationEntity = cachedEntities.FirstOrDefault(e => e.Entity.Token == token);

                if (registrationEntity?.Entity == null || 
                    registrationEntity.Entity.ExpiredDate < DateTime.UtcNow)
                {
                    var error = $"Deny confirmation with token : {token}.";
                    _logger.LogInformation(error);
                    throw new ApiException(new Error
                    {
                        ErrorMessage = error
                    });
                }

                _logger.LogInformation($"Performed confirmation for : {registrationEntity.Entity.AffiliateId}.");

                var response = await _affiliateService.SetAffiliateStateAsync(new SetAffiliateStateRequest()
                {
                    AffiliateId = registrationEntity.Entity.AffiliateId,
                    State = State.Active
                });
                
                this.ProcessResult(response);
                
                await _dataWriter.DeleteAsync(registrationEntity.PartitionKey, registrationEntity.RowKey);
                return RedirectPermanent(Program.Settings.ConfirmationRedirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured while getting confirmation.");
                throw;
            }
        }
    }
}