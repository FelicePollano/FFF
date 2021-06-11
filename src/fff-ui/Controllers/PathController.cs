using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace fff_ui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PathController : ControllerBase
    {
        public  string[] Get([FromBody] string basePath)
        {
            if(string.IsNullOrEmpty(basePath))
            {
                return Directory.EnumerateDirectories().ToArray();
            }
            else
            {
                return Directory.EnumerateDirectories(basePath).ToArray();
            }
        }
    }
}