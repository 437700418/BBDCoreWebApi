using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            services.AddAuthentication("Bearer").AddJwtBearer();
            services.AddAuthentication("Bearer").AddJwtBearer();
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
                Console.WriteLine("4 end");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

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
