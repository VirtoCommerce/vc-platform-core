using System.Data.Entity.Migrations;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<PlatformRepository>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Repositories\Migrations";
        }

        protected override void Seed(PlatformRepository context)
        {
            context.AddOrUpdate(new AccountEntity
            {
                Id = "1eb2fa8ac6574541afdb525833dadb46",
                UserName = "admin",
                IsAdministrator = true,
                UserType = AccountType.Administrator.ToString(),
                AccountState = AccountState.Approved.ToString(),
            });

            var frontendAccount = new AccountEntity
            {
                Id = "9b605a3096ba4cc8bc0b8d80c397c59f",
                UserName = "frontend",
                IsAdministrator = true,
                UserType = AccountType.Administrator.ToString(),
                AccountState = AccountState.Approved.ToString(),
            };
            frontendAccount.ApiAccounts.Add(new ApiAccountEntity
            {
                Name = "Frontend",
                ApiAccountType = ApiAccountType.Hmac,
                AccountId = frontendAccount.Id,
                AppId = "27e0d789f12641049bd0e939185b4fd2",
                SecretKey = "34f0a3c12c9dbb59b63b5fece955b7b2b9a3b20f84370cba1524dd5c53503a2e2cb733536ecf7ea1e77319a47084a3a2c9d94d36069a432ecc73b72aeba6ea78",
                IsActive = true,
            });
            context.AddOrUpdate(frontendAccount);

            context.SaveChanges();
        }
    }
}
