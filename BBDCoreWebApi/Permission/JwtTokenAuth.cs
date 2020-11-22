using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.IdentityModel.Tokens.Jwt;

namespace Blog.Core.AuthHelper
{
    /// <summary>
    /// 中间件
    /// 原做为自定义授权中间件
    /// 先做检查 header token的使用
    /// </summary>
    public class JwtTokenAuth
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// 管道执行到该中间件时候下一个中间件的RequestDelegate请求委托，如果有其它参数，也同样通过注入的方式获得
        /// </summary>
        /// <param name="next"></param>
        public JwtTokenAuth(RequestDelegate next)
        {
            //通过注入方式获得对象
            _next = next;
        }


        /// <summary>
        /// 处理前
        /// </summary>
        /// <param name="next"></param>
        private void PreProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke preproceed");
            //...
        }


        private void PostProceed(HttpContext next)
        {
            //Console.WriteLine($"{DateTime.Now} middleware invoke postproceed");
            //....
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            PreProceed(httpContext);


            //检测是否包含'Authorization'请求头
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                PostProceed(httpContext);

                return _next(httpContext);
            }
            //但是！请注意，这个时候我们输入了 token，我们就会在 httpcontext 上下文中，添加上我们自己自定义的身份验证方案！！！
            //这就是没有继续报错的根本原因
            var tokenHeader = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {

                //授权
                var claimList = new List<Claim>();
                var claim = new Claim(ClaimTypes.Role, "Admins");
                claimList.Add(claim);

                // 这里应该从 token 中读取，这里直接写死了
                var claims = new Claim[]{
                    new Claim(ClaimTypes.Name, "laozhang"),
                    new Claim(JwtRegisteredClaimNames.Email, "laozhang@qq.com"),
                    new Claim(JwtRegisteredClaimNames.Sub, "1"),//主题subject，就是id uid
                };

                var identity = new ClaimsIdentity(claims);
                var principal = new ClaimsPrincipal(identity);
                httpContext.User = principal;

            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now} middleware wrong:{e.Message}");
            }


            PostProceed(httpContext);

            //将上下文传递给下一个中间件
            return _next(httpContext);
        }

    }

}