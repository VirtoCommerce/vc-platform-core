using Microsoft.AspNetCore.Mvc;
using Module1.Data.Services;

namespace Module1.Web.Controllers
{
    [Route("api/[controller]")]
    public class ThirdPartyController
    {
        public ThirdPartyController()
        {
        }

        // GET api/thirdparty
        [HttpGet]
        public string Execute3rdPartyCodeFromDifferentProject(string message)
        {
            return new ThirdPartyServiceImpl().CallThirdPartyMethodFromAnotherProject(message);
        }
    }
}
