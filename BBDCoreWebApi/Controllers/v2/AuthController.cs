using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BBDCore.Common.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBDCoreWebApi.Controllers.v2
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpGet]
        public object GetToken()
        {
            string jwtToken = JwtHelper.IssueJwt(new TokenModelJwt()
            {
                Role = "manage,system,guest",
                Uid = 123,
                Work = "Work",
            });
            return Ok(jwtToken);
        }

        [Authorize]//授权
        [HttpGet]

        public object GetTokenInfo()
        {
            return new JsonResult(HttpContext.User.Identity.IsAuthenticated);
        }

        [HttpGet, Authorize(Policy = "guest")]
        public string ValidateGuest() {

            return "通过guest验证";
        }


        [HttpGet,Authorize(Policy = "user")]
        public string ValidateUser()
        {
            return "通过user验证";
        }
    }
}