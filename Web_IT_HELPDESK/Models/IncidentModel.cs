using System;
using System.Linq;

namespace Web_IT_HELPDESK.Models
{
    public class IncidentModel : Incident
    {
        public string userCreateName { get; set; }
        public string userResolveName { get; set; }
        public string statusName { get; set; }
        public string levelName { get; set; }
        public string departmentName { get; set; }
        public string plantName { get; set; }
    }
}