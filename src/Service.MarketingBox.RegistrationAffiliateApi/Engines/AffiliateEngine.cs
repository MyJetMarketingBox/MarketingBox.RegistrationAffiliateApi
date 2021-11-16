using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.MarketingBox.RegistrationAffiliateApi.Domain.Models;

namespace Service.MarketingBox.RegistrationAffiliateApi.Engines
{
    public class AffiliateEngine
    {
        private readonly ILogger<AffiliateEngine> _logger;
        private readonly IAffiliateService _affiliateService;
        private readonly EmailEngine _emailEngine;
        private readonly IMyNoSqlServerDataWriter<AffiliateRegistrationNoSql> _registrationDataWriter;

        public AffiliateEngine(IAffiliateService affiliateService, 
            IMyNoSqlServerDataWriter<AffiliateRegistrationNoSql> registrationDataWriter,
            EmailEngine emailEngine, 
            ILogger<AffiliateEngine> logger)
        {
            _affiliateService = affiliateService;
            _registrationDataWriter = registrationDataWriter;
            _emailEngine = emailEngine;
            _logger = logger;
        }

        public async Task Registration(string username, string password, string email)
        {
            var token = Guid.NewGuid().ToString("N");
            var noSqlEntity = AffiliateRegistrationNoSql.Create(new AffiliateRegistration()
            {
                RequestDate = DateTime.UtcNow,
                Email = email,
                Username = username,
                Password = password,
                Token = token
            });
            try
            {
                await _emailEngine.SendRegEmailAsync(email, token);
                await _registrationDataWriter.InsertOrReplaceAsync(noSqlEntity);
                await _registrationDataWriter.CleanAndKeepMaxPartitions(100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public async Task<bool> Confirmation(string token)
        {
            var cachedEntities = await _registrationDataWriter.GetAsync();
            var registrationEntity = cachedEntities.FirstOrDefault(e => e.Registration.Token == token);
            
            if (registrationEntity != null)
            {
                if (registrationEntity.Registration.RequestDate.Date != DateTime.UtcNow.Date)
                {
                    _logger.LogInformation($"Deny confirmation for : {registrationEntity.Registration.Email}.");
                    return false;
                }
                
                // TODO: process affiliate
                
                _logger.LogInformation($"Performed confirmation for : {registrationEntity.Registration.Email}.");
                await _registrationDataWriter.DeleteAsync(registrationEntity.PartitionKey, registrationEntity.RowKey);
                return true;
            }
            
            _logger.LogInformation($"Cannot find token :{token}.");
            return false;
        }
    }
}