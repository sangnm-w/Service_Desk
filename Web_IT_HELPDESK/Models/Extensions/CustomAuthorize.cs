using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web_IT_HELPDESK.Models.Extensions
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        ServiceDeskEntities en = new ServiceDeskEntities();
        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            bool isAdmin = ApplicationUser.Instance.isAdmin;
            if (isAdmin)
                return true;

            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            string currentArea = rd.Values["area"] as string;

            List<Role> userRoles = ApplicationUser.Instance.GetRoles();

            if (userRoles.Count() <= 0)
                return false;

            List<Rule> validRules = ApplicationUser.Instance.GetRules(userRoles);

            if (validRules.Count() <= 0)
                return false;

            foreach (var rule in validRules.Distinct())
            {
                if (rule.Rule_Name.ToUpper() == currentAction.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Controller.TempData["UnAuthorizedMsg"] = "You do not have sufficient permissions to perform this operation.";
                filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
        }
    }
}