using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Web_IT_HELPDESK.Commons
{
    public class ModuleConstant
    {
        public static string INCIDENT = "INCIDENT";
        public static string DEVICE = "DEVICE";
        public static string DEVICE_PROVIDING = "DEVICE_PROVIDING";
        public static string CONTRACT = "CONTRACT";
        public static string BIZ_TRIP = "BIZ_TRIP";
    }

    public class ActionConstant
    {
        public static string INDEX = "INDEX";
        public static string DETAILS = "DETAILS";
        public static string CREATE = "CREATE";
        public static string DELETE = "DELETE";
        public static string EDIT = "EDIT";
        public static string SOLVE = "SOLVE";
        public static string DOWNLOAD = "DOWNLOAD";
        public static string UPLOAD = "UPLOAD";
    }

    public class RoleConstant
    {
        public static string Administrator = "Administrator";
    }
}