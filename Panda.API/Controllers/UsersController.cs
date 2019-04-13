using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Panda.API.Models;

namespace Panda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Authorize]
        public ActionResult<User> Get()
        {
            return (User)HttpContext.Items["ApplicationUser"];
        }
    }
}
