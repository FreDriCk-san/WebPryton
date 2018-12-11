using System;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebPryton.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET api/User/bool
        [HttpGet("bool")]
        public bool GetUserBool()
        {
            return true;
        }

        // GET api/User/int
        [HttpGet("int")]
        public int GetUserInt()
        {
            return 123;
        }

        // GET api/User/string
        [HttpGet("string")]
        public string GetUserString()
        {
            return "Hello";
        }

        // GET api/User/stringEmpty
        [HttpGet("stringEmpty")]
        public string GetUserStringEmpty()
        {
            return String.Empty;
        }

        // GET api/User/null
        [HttpGet("null")]
        public object GetUserNull()
        {
            return null;
        }

        // GET api/User/object
        [HttpGet("object")]
        public object GetUserObject()
        {
            var obj = new Object();
            obj = new string[] { "user1", "user2", "user3" };
            return obj;
        }


        // GET api/User/badRequest
        [HttpGet("badRequest")]
        public ActionResult GetUserBadRequest()
        {
            return BadRequest("User bad request");
        }

        // GET api/User/serverError
        [HttpGet("serverError")]
        public ActionResult GetUserServerError()
        {
            throw new Exception("Internal Server Error was occured!");
        }

        // GET api/User/forbidden
        [HttpGet("forbidden")]
        public ActionResult GetUserForbidden()
        {
            return StatusCode(403, "User was forbidden");
        }

        // GET api/User/unauthorized
        [HttpGet("unauthorized")]
        public ActionResult GetUserUnauthorizedt()
        {
            return StatusCode(401, "User was unauthorized!");
        }
    }
}
