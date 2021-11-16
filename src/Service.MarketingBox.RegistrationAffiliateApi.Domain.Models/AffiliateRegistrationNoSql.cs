using MyNoSqlServer.Abstractions;

namespace Service.MarketingBox.RegistrationAffiliateApi.Domain.Models
{
    public class AffiliateRegistrationNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "affiliate-registration";
        private static string GeneratePartitionKey() => $"Registration";
        private static string GenerateRowKey(string email) => email;
        public AffiliateRegistration Registration { get; set; }
        
        public static AffiliateRegistrationNoSql Create(AffiliateRegistration entity)
        {
            return new AffiliateRegistrationNoSql()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(entity.Email),
                Registration = entity
            };
        }
    }
}