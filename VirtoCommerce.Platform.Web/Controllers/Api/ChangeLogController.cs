using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Web.Controllers.Api
{
    [Route("api/platform/changelog")]
    public class ChangeLogController : Controller
    {
        private readonly IChangeLogService _changeLog;
        public ChangeLogController(IChangeLogService changeLog)
        {
            _changeLog = changeLog;
        }

        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(OperationLog[]), 200)]
        public IActionResult SearchObjectChangeHistory([FromBody]TenantIdentity tenant)
        {
            var result = _changeLog.FindObjectChangeHistory(tenant.Id, tenant.Type).ToArray();
            return Ok(result);
        }

        [HttpGet]
        [Route("{type}/changes")]
        [ProducesResponseType(typeof(OperationLog[]), 200)]
        public IActionResult SearchTypeChangeHistory(string type, [FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null)
        {
            var result = _changeLog.FindChangeHistory(type, start, end).ToArray();
            return Ok(result);
        }
    }
}
