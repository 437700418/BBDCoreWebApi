using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBDCore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBDCoreWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class BlogController : ControllerBase
    {
        /// <summary>
        /// 获取博客信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet,Authorize]
        public IActionResult GetBlog(int Id)
        {
            return new JsonResult($"第{Id}篇博客");
        }

        /// <summary>
        /// 博客提交接口
        /// </summary>
        /// <param name="love">model实体类参数</param>
        [HttpPost]
        public IActionResult Post(Love love)
        {
            return new JsonResult(love);
        }
    }
}