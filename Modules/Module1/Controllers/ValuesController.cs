using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Modules.Abstractions;
using Module1.Services;
using Module1.Abstractions;

namespace Module2.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IModuleCatalog _moduleCatalog;
        private readonly IMyService _myService;
        public ValuesController(IModuleCatalog moduleCatalog, IMyService myService)
        {
            _moduleCatalog = moduleCatalog;
            _myService = myService;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _moduleCatalog.Modules.Select(x => x.ModuleName);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return _myService.GetValues();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
