using System.Web;
using System.Web.Mvc;

namespace Web_IT_HELPDESK
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}