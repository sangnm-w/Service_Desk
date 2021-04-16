using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Web_IT_HELPDESK.Commons
{
    public class ModuleConstant
    {
        public const string INCIDENT = "INCIDENT";
        public const string DEVICE = "DEVICE";
        public const string DEVICE_PROVIDING = "DEVICE_PROVIDING";
        public const string CONTRACT = "CONTRACT";
        public const string BIZ_TRIP = "BIZ_TRIP";
    }

    public class ActionConstant
    {
        public const string INDEX = "INDEX";
        public const string DETAILS = "DETAILS";
        public const string CREATE = "CREATE";
        public const string DELETE = "DELETE";
        public const string EDIT = "EDIT";
        public const string SOLVE = "SOLVE";
        public const string DOWNLOAD = "DOWNLOAD";
        public const string UPLOAD = "UPLOAD";
    }

    public class RoleConstant
    {
        public static string Administrator = "Administrator";
    }
}