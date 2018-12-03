using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebPryton.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        // GET: api/User
        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return new string[] { "user1", "user2" };
        }

        // GET api/User/5
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok($"user {id}");
        }

        // POST api/User/Create
        [HttpPost("Create")]
        public IActionResult CreateUser([FromBody]string value)
        {
            return Ok("Created:"+value);
        }

        // PUT api/User/5
        [HttpPut("{id}")]
        public void UpdateUser(int id, [FromBody]string value)
        {
        }

        // DELETE api/User/5
        [HttpDelete("{id}")]
        public void DeleteUser(int id)
        {
        }
    }
}
