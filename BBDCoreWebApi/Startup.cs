using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBDCore.Common.Helper;
using BBDCoreWebApi.Extensions;
using BBDCoreWebApi.Mildd;
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
            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));//密钥
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//加密方式

            services.AddSingleton<HttpContextAccessor, HttpContextAccessor>();
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
                RequireExpirationTime=true,
            };
            services.AddAuthentication("Bearer").AddJwtBearer((a) => 
            {
                a.TokenValidationParameters = tokenValidationParameters;
            });
            services.AddSwaggerSetup();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseAuthentication();

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
                Console.WriteLine("4 end");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"{ApiName} V1");

                //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,
                //注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
                c.RoutePrefix = "";
            });

            app.UseSwaggerMildd();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
