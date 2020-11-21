using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDCore.Model;
using BBDCoreWebApi.SwaggerHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BBDCoreWebApi.Extensions.CustomApiVersion;

namespace BBDCoreWebApi.Controllers.v2
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class BlogController : ControllerBase
    {
        /// <summary>
        /// 获取博客测试信息 v2版本
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        ////MVC自带特性 对 api 进行组管理
        //[ApiExplorerSettings(GroupName = "v2")]
        ////路径 如果以 / 开头，表示绝对路径，反之相对 controller 的想u地路径
        //[Route("/api/v2/blog/Blogtest")]
        //和上边的版本控制以及路由地址都是一样的
        [CustomRoute(ApiVersions.v2, "Blogtest")]
        public object Blogtest()
        {
            return Ok(new { status = 220, data = "我是第二版的博客信息" });
        }


    }
}
