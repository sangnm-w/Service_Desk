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

            List<Role> validRoles = new List<Role>();

            foreach (var role in allowedroles)
            {
                var ro = userRoles.FirstOrDefault(r => r.Role_Name.ToUpper() == role.ToUpper());
                validRoles.Add(ro);
            }

            if (validRoles.Count() <= 0)
                return false;

            List<Rule> validRules = ApplicationUser.Instance.GetRules(validRoles);

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
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    bool authorize = false;

        //    var rd = httpContext.Request.RequestContext.RouteData;
        //    string currentAction = rd.GetRequiredString("action");
        //    string currentController = rd.GetRequiredString("controller");
        //    string currentArea = rd.Values["area"] as string;

        //    var rolesInAuthorizeByUserId = en.Authorizations
        //                    .Where(au => au.Emp_CJ == httpContext.User.Identity.Name)
        //                    .Join(en.Roles, au => au.Role_ID, ro => ro.Role_ID, (au, ro) => new { au, ro })
        //                    .Where(grp => grp.ro.Deactive != true)
        //                    .Select(grp => grp.ro);

        //    if (rolesInAuthorizeByUserId.Count() <= 0)
        //        return authorize;

        //    List<Role> validRoles = new List<Role>();

        //    foreach (var role in allowedroles)
        //    {
        //        var ro = rolesInAuthorizeByUserId.FirstOrDefault(r => r.Role_Name.ToUpper() == role.ToUpper());
        //        validRoles.Add(ro);
        //    }

        //    if (validRoles.Count() <= 0)
        //        return authorize;

        //    List<Rule> validRules = new List<Rule>();

        //    foreach (var role in validRoles)
        //    {
        //        var ruleByValidRoleId = en.Roles.Where(ro => ro.Role_ID == role.Role_ID).SelectMany(ro => ro.Rules);
        //        ruleByValidRoleId = ruleByValidRoleId.Where(ru => ru.Deactive != true);
        //        validRules.AddRange(ruleByValidRoleId);
        //    }

        //    if (validRules.Count() <= 0)
        //        return authorize;

        //    foreach (var rule in validRules.Distinct())
        //    {
        //        if (rule.Rule_Name.ToUpper() == currentAction.ToUpper())
        //        {
        //            authorize = true;
        //            return authorize;
        //        }
        //    }

        //    return authorize;
        //}

        //protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        //{
        //    actionContext.Response = new HttpResponseMessage
        //    {
        //        StatusCode = HttpStatusCode.Forbidden,
        //        Content = new StringContent("You are unauthorized to access this resource")
        //    };
        //}

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            
            filterContext.Result = new RedirectToRouteResult(
                         new RouteValueDictionary(
                             new
                             {
                                 controller = "LogOn",
                                 action = "Logon",
                                 returnUrl = filterContext.HttpContext.Request.Url.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped)
                             })
                         );
        }
    }
}