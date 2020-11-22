using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDCore.Common.Helper;
using BBDCoreWebApi.Extensions;
using BBDCoreWebApi.Mildd;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BBDCoreWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string ApiName { get; set; } = "BBDCoreWebApi";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Appsettings(Configuration));
            services.AddControllers();

            var basePath = ApplicationEnvironment.ApplicationBasePath;
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = AppSecretConfig.Audience_Secret_String;
            //��Կ (SymmetricSecurityKey �԰�ȫ�Ե�Ҫ����Կ�ĳ���̫�̻ᱨ���쳣)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));//��Կ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//���ܷ�ʽ

            services.AddSingleton<HttpContextAccessor, HttpContextAccessor>();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,//�Ƿ���֤SecurityKey
                ValidateIssuerSigningKey = true,//�Ƿ���֤Issuer��
                ValidateAudience = true,//�Ƿ���֤Audience
                ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                IssuerSigningKey = key,
                ValidIssuer = iss,
                ValidAudience = aud,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };


            // 1����Ȩ����������ϱߵ�����ͬ�����ô����ǲ�����controller�У�д��� roles ��
            // Ȼ����ôд [Authorize(Policy = "Admin")]
            //���ýӿ���Ȩ����
            services.AddAuthorization(options =>
            {
                options.AddPolicy("guest", policy => policy.RequireRole(Role.guest.ToString()).Build());

                options.AddPolicy("admin", policy => policy.RequireRole(Role.manage.ToString(), Role.system.ToString()).Build());

                options.AddPolicy("user", policy => policy.RequireRole(Role.user.ToString()));
            });

            services.AddAuthentication("Bearer").AddJwtBearer((a) =>
            {
                a.TokenValidationParameters = tokenValidationParameters;
                a.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddSwaggerSetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                Console.WriteLine("1 start");
                await next();
                Console.WriteLine("1 end");
            });

            app.Use(async (context, next) =>
            {
                Console.WriteLine("2 start");
                await next();
                Console.WriteLine("2 end");
            });

            app.Use(async (context, next) =>
            {
                Console.WriteLine("3 start");
                await next();
                Console.WriteLine("3 end");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwaggerMildd();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
