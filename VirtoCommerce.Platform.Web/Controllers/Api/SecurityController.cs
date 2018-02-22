using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using OpenIddict.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Search;
using VirtoCommerce.Platform.Web.Model.Security;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/platform/security")]
    public class SecurityController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SecurityOptions _securityOptions;
        private readonly IPermissionsProvider _permissionsProvider;
        private readonly IUserSearchService _userSearchService;
        private readonly IRoleSearchService _roleSearchService;

        public SecurityController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager,
                IPermissionsProvider permissionsProvider, IUserSearchService userSearchService, IRoleSearchService roleSearchService,
                IOptions<SecurityOptions> securityOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _securityOptions = securityOptions.Value;
            _permissionsProvider = permissionsProvider;
            _roleManager = roleManager;
            _userSearchService = userSearchService;
            _roleSearchService = roleSearchService;
        }

        /// <summary>
        /// Sign in with user name and password
        /// </summary>
        /// <remarks>
        /// Verifies provided credentials and if succeeded returns full user details, otherwise returns 401 Unauthorized.
        /// </remarks>
        /// <param name="model">User credentials.</param>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Identity.SignInResult), 200)]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody]LoginRequest request)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, true);
            if (loginResult.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.UserName);
                //Do not allow login to admin customers and rejected users
                if (await _signInManager.UserManager.IsInRoleAsync(user, SecurityConstants.Roles.Customer))
                {
                    loginResult = Microsoft.AspNetCore.Identity.SignInResult.NotAllowed;
                }
            }
            return Ok(loginResult);
        }


        /// <summary>
        /// Sign out
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Get current user details
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("currentuser")]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(UserDetail), 200)]
        public async Task<ActionResult> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }
            var result = new UserDetail
            {
                Id = user.Id,
                isAdministrator = await _userManager.IsInRoleAsync(user, SecurityConstants.Roles.Administrator),
                UserName = user.UserName,

            };
            var roleNames = await _userManager.GetRolesAsync(user);
            foreach(var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                result.Permissions.AddRange((await _roleManager.GetClaimsAsync(role)).Where(x=>x.Type.EqualsInvariant(SecurityConstants.Claims.PermissionClaimType)).Select(x => x.Value));
            }
            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [Route("userinfo")]
        [ProducesResponseType(typeof(Claim[]), 200)]
        public async Task<IActionResult> Userinfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user profile is no longer available."
                });
            }

            var claims = new JObject();

            //TODO: replace to PrinciplaClaims

            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            claims[OpenIdConnectConstants.Claims.Subject] = await _userManager.GetUserIdAsync(user);

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Email))
            {
                claims[OpenIdConnectConstants.Claims.Email] = await _userManager.GetEmailAsync(user);
                claims[OpenIdConnectConstants.Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Phone))
            {
                claims[OpenIdConnectConstants.Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
                claims[OpenIdConnectConstants.Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIddictConstants.Scopes.Roles))
            {
                claims["roles"] = JArray.FromObject(await _userManager.GetRolesAsync(user));
            }

            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return Json(claims);
        }

        /// <summary>
        /// Get all registered permissions
        /// </summary>
        [HttpGet]
        [Route("permissions")]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        [ProducesResponseType(typeof(Permission[]), 200)]
        public ActionResult GetAllRegisteredPermissions()
        {
            var result = _permissionsProvider.GetAllPermissions().ToArray();
            return Ok(result);
        }

        /// <summary>
        /// Search roles by keyword
        /// </summary>
        /// <param name="request">Search parameters.</param>
        [HttpPost]
        [Route("roles")]
        [ProducesResponseType(typeof(GenericSearchResult<Role>), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> SearchRoles(RoleSearchCriteria request)
        {
            var result = await _roleSearchService.SearchRolesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        /// <param name="roleId"></param>
        [HttpGet]
        [Route("roles/{roleId}")]
        [ProducesResponseType(typeof(Role), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> GetRoleAsync(string roleId)
        {
            var result = await _roleManager.FindByIdAsync(roleId);
            return Ok(result);
        }

        /// <summary>
        /// Delete roles by ID
        /// </summary>
        /// <param name="roleIds">An array of role IDs.</param>
        [HttpDelete]
        [Route("roles")]
        [ProducesResponseType(200)]
        [Authorize(SecurityConstants.Permissions.SecurityDelete)]
        public async Task<ActionResult> DeleteRolesASync([FromQuery(Name = "ids")] string[] roleIds)
        {
            if (roleIds != null)
            {
                foreach (var roleId in roleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        await _roleManager.DeleteAsync(role);
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// Add a new role or update an existing role
        /// </summary>
        /// <param name="role"></param>
        [HttpPut]
        [Route("roles")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult> UpdateRoleAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            return Ok(result);
        }

        //TODO: Implement generation new OAuth clients 

        ///// <summary>
        ///// Generate new API account
        ///// </summary>
        ///// <remarks>
        ///// Generates new account but does not save it.
        ///// </remarks>
        ///// <param name="type"></param>
        //[HttpGet]
        //[Route("apiaccounts/new")]
        //[ResponseType(typeof(ApiAccount))]
        //[CheckPermission(Permission = PredefinedPermissions.SecurityUpdate)]
        //public IHttpActionResult GenerateNewApiAccount(AuthenticationLoginProviderType type)
        //{
        //    var result = _securityService.GenerateNewApiAccount(type);
        //    result.IsActive = null;
        //    return Ok(result);
        //}

        ///// <summary>
        ///// Generate new API key for specified account
        ///// </summary>
        ///// <remarks>
        ///// Generates new key for specified account but does not save it.
        ///// </remarks>
        //[HttpPut]
        //[Route("apiaccounts/newKey")]
        //[ResponseType(typeof(void))]
        //[CheckPermission(Permission = PredefinedPermissions.SecurityUpdate)]
        //public IHttpActionResult GenerateNewApiKey(ApiAccount account)
        //{
        //    if (account.ApiAccountType != AuthenticationLoginProviderType.Hmac)
        //    {
        //        return BadRequest(SecurityResources.NonHmacKeyGenerationException);
        //    }
        //    var retVal = _securityService.GenerateNewApiKey(account);
        //    return Ok(retVal);
        //}

        /// <summary>
        /// Search users by keyword
        /// </summary>
        /// <param name="request">Search parameters.</param>
        [HttpPost]
        [Route("users")]
        [ProducesResponseType(typeof(GenericSearchResult<ApplicationUser>), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> SearchUsersAsync(UserSearchCriteria criteria)
        {
            var result = await _userSearchService.SearchUsersAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Get user details by user name
        /// </summary>
        /// <param name="userName"></param>
        [HttpGet]
        [Route("users/{userName}")]
        [ProducesResponseType(typeof(ApplicationUser), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> GetUserByNameAsync(string userName)
        {
            var retVal = await _userManager.FindByNameAsync(userName);
            return Ok(retVal);
        }
       

        /// <summary>
        /// Get user details by user ID
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("users/id/{id}")]
        [ProducesResponseType(typeof(ApplicationUser), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> GetUserByIdAsync(string id)
        {
            var retVal = await _userManager.FindByIdAsync(id);
            return Ok(retVal);
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="newUserRequest"></param>
        [HttpPost]
        [Route("users/create")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityCreate)]
        public async Task<ActionResult> CreateAsync(RegisterUserRequest newUserRequest)
        {
            var user = new ApplicationUser
            {
                UserName = newUserRequest.UserName
            };
            var result = await _userManager.CreateAsync(user, newUserRequest.Password);
            return Ok(result);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="changePassword">Old and new passwords.</param>
        [HttpPost]
        [Route("users/{userName}/changepassword")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult> ChangePasswordAsync(string userName, [FromBody] ChangePasswordRequest changePassword)
        {
            if(!IsUserEditable(userName))
            {
                return BadRequest(new IdentityError() { Description = "It is forbidden to edit this user." });
            }
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                return BadRequest(IdentityResult.Failed(new IdentityError() { Description = "User not found" }));             
            }

            var result = await _signInManager.UserManager.ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);
            return Ok(result);
        }

        /// <summary>
        /// Reset password confirmation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resetPasswordConfirm">New password.</param>
        [HttpPost]
        [Route("users/{userId}/resetpasswordconfirm")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult> ResetPassword(string userId, [FromBody] ResetPasswordConfirmRequest resetPasswordConfirm)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(IdentityResult.Failed(new IdentityError() { Description = "User not found" }));
            }
            if (!IsUserEditable(user.UserName))
            {
                return BadRequest(new IdentityError() { Description = "It is forbidden to edit this user." });
            }
            var result = await _signInManager.UserManager.ResetPasswordAsync(user, resetPasswordConfirm.Token, resetPasswordConfirm.NewPassword);
            return Ok(result);
        }

    
        /// <summary>
        /// Send email with instructions on how to reset user password.
        /// </summary>
        /// <remarks>
        /// Verifies provided userName and (if succeeded) sends email.
        /// </remarks>
        [HttpPost]
        [Route("users/{loginOrEmail}/requestpasswordreset")]
        [ProducesResponseType(200)]
        [AllowAnonymous]
        public async Task<ActionResult> RequestPasswordReset(string loginOrEmail)
        {
            var user = await _userManager.FindByNameAsync(loginOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginOrEmail);
            }
            
            //Do not permit rejected users and customers
            if (user != null && user.Email != null && IsUserEditable(user.UserName) && !(await _userManager.IsInRoleAsync(user, SecurityConstants.Roles.Customer)))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{Request.Scheme}{Request.Host}/api/platform/security/#/resetpassword/{user.Id}/{token}";

                //TODO: Generate Domain Event and implement the sending password reset email to user in event handler

                //var notification = _notificationManager.GetNewNotification<ResetPasswordEmailNotification>("Platform", typeof(ResetPasswordEmailNotification).Name, "en");
                //notification.Url = $"{uri}/#/resetpassword/{user.Id}/{token}";
                //notification.Recipient = user.Email;
                //notification.Sender = "noreply@" + Request.RequestUri.Host;
                //try
                //{
                //    var result = _notificationManager.SendNotification(notification);
                //    retVal.Succeeded = result.IsSuccess;
                //    if (!retVal.Succeeded)
                //    {
                //        retVal.Errors = new string[] { result.ErrorMessage };
                //    }
                //}
                //catch (Exception ex)
                //{
                //    //Display errors only when sending notifications fail
                //    retVal.Errors = new string[] { ex.Message };
                //    retVal.Succeeded = false;
                //}
                return Ok(callbackUrl);
            }

            return Ok();
        }

        /// <summary>
        /// Update user details by user ID
        /// </summary>
        /// <param name="user">User details.</param>
        [HttpPut]
        [Route("users")]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult> UpdateAsync(ApplicationUser user)
        {
            if(user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (!IsUserEditable(user.UserName))
            {
                return BadRequest(new IdentityError() { Description = "It is forbidden to edit this user." });
            }
            var result = await _userManager.UpdateAsync(user);
            return Ok(result);
        }

        /// <summary>
        /// Delete users by name
        /// </summary>
        /// <param name="names">An array of user names.</param>
        [HttpDelete]
        [Route("users")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityDelete)]
        public async Task<ActionResult> DeleteAsync([FromQuery] string[] names)
        {
            if (names == null)
            {
                throw new ArgumentNullException(nameof(names));
            }
            if (names.Any(x=> !IsUserEditable(x)))
            {
                return BadRequest(new IdentityError() { Description = "It is forbidden to edit these users." });
            }
            foreach (var userName in names)
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result);
                    }
                }
            }
            return Ok(IdentityResult.Success);
        }

        /// <summary>
        /// Checks if user locked
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("users/{id}/locked")]
        [ProducesResponseType(typeof(UserLockedResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityQuery)]
        public async Task<ActionResult> IsUserLockedAsync(string id)
        {
            var result = new UserLockedResult(false);
            var user = await _userManager.FindByIdAsync(id);
            if(user != null)
            {
                result.Locked = await _userManager.IsLockedOutAsync(user);
            }         
            return Ok(result);
        }

        /// <summary>
        /// Unlock user
        /// </summary>
        /// <param name="id">>User id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/{id}/unlock")]
        [ProducesResponseType(typeof(IdentityResult), 200)]
        [Authorize(SecurityConstants.Permissions.SecurityUpdate)]
        public async Task<ActionResult> UnlockUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
                return Ok(result);
            }
            return Ok(IdentityResult.Failed());
        }


        private bool IsUserEditable(string userName)
        {
            return _securityOptions.NonEditableUsers?.FirstOrDefault(x => x.EqualsInvariant(userName)) != null;
        }

     
    }
}
