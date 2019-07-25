using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExportModule.Test.MockHelpers
{
    /// <summary>
    /// Helps to mock basic authorization services
    /// </summary>
    public static class AuthMockHelper
    {
        /// <summary>
        /// Helps to mock basic authorization services
        /// </summary>
        /// <param name="authorizationResult"><see cref="true"/> to mock successful authorization, <see cref="false"/> to non-successful.</param>
        /// <returns></returns>
        public static (IAuthorizationService AuthorizationService,
            IAuthorizationPolicyProvider AuthorizationPolicyProvider,
            IUserClaimsPrincipalFactory<ApplicationUser> UserClaimsPrincipalFactory,
            UserManager<ApplicationUser> UserManager)
            AuthServicesMock(bool authorizationResult)
        {
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            authorizationServiceMock.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).ReturnsAsync(authorizationResult ? AuthorizationResult.Success() : AuthorizationResult.Failed(AuthorizationFailure.Failed(new IAuthorizationRequirement[] { new Mock<IAuthorizationRequirement>().Object })));

            var authorizationPolicyProviderMock = new Mock<IAuthorizationPolicyProvider>();
            authorizationPolicyProviderMock.Setup(x => x.GetPolicyAsync(It.IsAny<string>())).ReturnsAsync(new AuthorizationPolicy(new IAuthorizationRequirement[] { new Mock<IAuthorizationRequirement>().Object }, new string[] { }));

            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            return (authorizationServiceMock.Object, authorizationPolicyProviderMock.Object, userClaimsPrincipalFactoryMock.Object, userManagerMock.Object);
        }
    }
}
