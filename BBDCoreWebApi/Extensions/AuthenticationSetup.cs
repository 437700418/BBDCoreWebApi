using BBDCore.Common.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDCoreWebApi.Extensions
{
    public static class AuthenticationSetup
    {
        public static void AddAuthenticationSetup(this IServiceCollection services) {

            var basePath = ApplicationEnvironment.ApplicationBasePath;
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = AppSecretConfig.Audience_Secret_String;
            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));//密钥
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//加密方式

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,//是否验证SecurityKey
                ValidateIssuerSigningKey = true,//是否验证Issuer、
                ValidateAudience = true,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间
                IssuerSigningKey = key,
                ValidIssuer = iss,
                ValidAudience = aud,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };


            services.AddAuthentication("Bearer").AddJwtBearer((a) =>
            {
                a.TokenValidationParameters = tokenValidationParameters;
                a.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，则把<是否过期>添加到，返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
