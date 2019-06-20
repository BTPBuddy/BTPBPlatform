using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTPBPlatform.Models
{
    public class AuthenticateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            ISession session = filterContext.HttpContext.Session;
            Controller controller = filterContext.Controller as Controller;

            if (controller != null)
            {
                if (!SessionUtils.SessionAuthenticated(session))
                {
                    filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary { { "Area", "" }, {"Controller", "Login" },
                                      { "Action", "Logout" } });
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
