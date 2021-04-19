using System.ComponentModel;

namespace Web_IT_HELPDESK.ViewModels
{
    public class IncidentViewModel : Incident
    {
        [DisplayName("Requester")]
        public string userCreateName { get; set; }

        [DisplayName("Solver")]
        public string userResolveName { get; set; }

        [DisplayName("Status")]
        public string statusName { get; set; }

        [DisplayName("Level")]
        public string levelName { get; set; }

        [DisplayName("Department")]
        public string departmentName { get; set; }

        public string plantId { get; set; }

        [DisplayName("Plant")]
        public string plantName { get; set; }
    }
}