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
                Role = "manage",
                Uid = 123,
                Work = "Work",
            });
            return Ok(jwtToken);
        }
        public object GetTokenInfo()
        {
            string jwtToken = JwtHelper.IssueJwt(new TokenModelJwt()
            {
                Role = "manage",
                Uid = 123,
                Work = "Work",
            });

            var user = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Jti);

            return new JsonResult(JwtHelper.ReadToken(jwtToken));
        }
    }
}