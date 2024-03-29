using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Airgap.WebUi.Controllers
{
    public class StatusCodeController : BaseController
    {
        // GET: /<controller>/
        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            return View(statusCode);
        }
    }
}