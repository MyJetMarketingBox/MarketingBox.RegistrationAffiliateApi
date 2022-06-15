using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Email.Service.Domain.Models;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.MarketingBox.RegistrationAffiliateApi.Controllers.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Controllers
{
    [ApiController]
    [Route("/api/affiliates")]
    public class AffiliatesController : ControllerBase
    {
        private readonly IAffiliateService _affiliateService;
        private readonly ILogger<AffiliatesController> _logger;
        private readonly IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> _dataWriter;
        private readonly ITokensService _tokensService;

        public AffiliatesController(IAffiliateService affiliateService,
            ILogger<AffiliatesController> logger,
            IMyNoSqlServerDataWriter<AffiliateConfirmationNoSql> dataWriter, 
            ITokensService tokensService)
        {
            _affiliateService = affiliateService;
            _logger = logger;
            _dataWriter = dataWriter;
            _tokensService = tokensService;
        }

        [HttpPost("registration")]
        public async Task<ActionResult<TokenInfo>> Registration(
            [Required, FromHeader(Name = "affiliate-id")]
            long affiliateId,
            [Required, FromHeader(Name = "api-key")]
            string apiKey,
            [FromBody] RegistrationRequest request)
        {
            _logger.LogInformation(
                $"AffiliatesController.Registration receive Haders: affiliateId - {affiliateId}, api-key - {apiKey}.");
            _logger.LogInformation("AffiliatesController.Registration receive request: {@Request}", request);

            request.ValidateEntity();
            
            var response = await _affiliateService.CreateSubAsync(new CreateSubRequest()
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                MasterAffiliateId = affiliateId,
                MasterAffiliateApiKey = apiKey,
                Phone = request.Phone,
                Sub = request.Sub
            });

            _logger.LogInformation("Get response from _affiliateService.CreateSubAsync: {@Reponse}", response);
            var aff = response.Process();
            var responseToken = await _tokensService.LoginAsync(new TokenRequest
            {
                Login = request.Email,
                Password = request.Password,
                TenantId = aff.TenantId
            });
            return responseToken.Process();
        }

        [HttpGet("confirmation/{token}")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Confirmation(
            [FromRoute, Required] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BadRequestException("Cannot process empty token");
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
                    throw new BadRequestException(error);
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