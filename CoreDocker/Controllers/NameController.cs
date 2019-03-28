using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CoreDocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NameController : ControllerBase
    {
        // GET api/name
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "shuanger" };
        }
    }
}
