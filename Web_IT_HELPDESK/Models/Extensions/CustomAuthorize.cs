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
        public ServiceDeskEntities en { get; set; }
        public ApplicationUser _appUser { get; set; }
        private string authorizeErr { get; set; } = null;

        private readonly string[] allowedroles;
        public CustomAuthorizeAttribute(params string[] roles)
        {
            en = new ServiceDeskEntities();
            _appUser = new ApplicationUser();
            allowedroles = roles;
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

            var rd = httpContext.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            string currentController = rd.GetRequiredString("controller");
            //string currentArea = rd.Values["area"] as string;

            Rule acceptedRule = null;

            acceptedRule = en.Rules
            .Join(en.Modules, ru => ru.Module_ID, mo => mo.Module_ID, (ru, mo) => new { ru, mo })
            .FirstOrDefault(grp => grp.ru.Rule_Name.ToUpper() == currentAction.ToUpper()
            && grp.mo.Module_Name.ToUpper() == currentController.ToUpper())?.ru;

            if (acceptedRule == null)
            {
                authorizeErr = "This module does not setup into Role and Rule. Please contact Mr. Sang - IT Software, email: minhsang.it@cjvina.com or zalo: 0937199707";
                return false;
            }

            bool isAdmin = _appUser.isAdmin;
            if (isAdmin)
                return true;

            List<Role> userRoles = _appUser.GetRoles();

            if (userRoles.Count() <= 0)
                return false;

            List<Rule> userRules = _appUser.GetRules(userRoles);

            if (userRules.Count() <= 0)
                return false;

            Rule userHasAcceptedRule = userRules.FirstOrDefault(ru => ru.Rule_ID == acceptedRule.Rule_ID && ru.Module_ID == acceptedRule.Module_ID);

            if (userHasAcceptedRule != null)
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else if (!string.IsNullOrWhiteSpace(authorizeErr))
            {
                filterContext.Controller.TempData["authorizeErr"] = authorizeErr;
                filterContext.Result = new RedirectToRouteResult(
                                    new RouteValueDictionary(new { controller = "Error", action = "Index" }));
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