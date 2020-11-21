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
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo()
            //    {
            //        // {ApiName} �����ȫ�ֱ����������޸�
            //        Version = "V1",
            //        Title = $"{ApiName} �ӿ��ĵ�����Netcore 3.1.4",
            //        Description = $"{ApiName} HTTP API V1",
            //        Contact = new OpenApiContact { Name = ApiName, Email = "437700418@qq.com", Url = new Uri("https://www.baidu.com") },
            //        License = new OpenApiLicense { Name = ApiName, Url = new Uri("https://www.baidu.com") }
            //    });
            //    //�����������������������
            //    var xmlPath = Path.Combine(basePath, "BBDCoreWebApi.xml");//������Ǹո����õ�xml�ļ���
            //    c.IncludeXmlComments(xmlPath, true);//Ĭ�ϵĵڶ���������false�������controller��ע�ͣ��ǵ��޸�

            //    var xmlModelPath = Path.Combine(basePath, "BBDCore.Model.xml");//�������Model���xml�ļ���
            //    c.IncludeXmlComments(xmlModelPath);

            //});
            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = AppSecretConfig.Audience_Secret_String;
            //��Կ (SymmetricSecurityKey �԰�ȫ�Ե�Ҫ����Կ�ĳ���̫�̻ᱨ���쳣)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));//��Կ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//���ܷ�ʽ

            services.AddSingleton<HttpContextAccessor, HttpContextAccessor>();
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = iss,
                ValidateAudience = true,
                ValidAudience = aud,
                ValidateLifetime = true,
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
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint($"/swagger/V1/swagger.json", $"{ApiName} V1");

            //    //·�����ã�����Ϊ�գ���ʾֱ���ڸ�������localhost:8001�����ʸ��ļ�,
            //    //ע��localhost:8001/swagger�Ƿ��ʲ����ģ�ȥlaunchSettings.json��launchUrlȥ����������뻻һ��·����ֱ��д���ּ��ɣ�����ֱ��дc.RoutePrefix = "doc";
            //    c.RoutePrefix = "";
            //});

            //app.UseSwaggerMildd();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
