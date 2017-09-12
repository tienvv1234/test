using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Airgap.WebUi.Controllers
{
    public class BaseController : Controller
    {
        public int current_UserID
        {
            get
            {
                if (HttpContext.Session.GetString("UserId") != null && Convert.ToInt16(HttpContext.Session.GetString("UserId")) > 0)
                {
                    return Convert.ToInt16(HttpContext.Session.GetString("UserId"));
                }
                return 0;
            }
        }

        public int current_ApplianceId
        {
            get
            {
                if (HttpContext.Session.GetString("ApplianceId") != null && Convert.ToInt16(HttpContext.Session.GetString("ApplianceId")) > 0)
                {
                    return Convert.ToInt16(HttpContext.Session.GetString("ApplianceId"));
                }
                return 0;
            }
        }

    }
}
