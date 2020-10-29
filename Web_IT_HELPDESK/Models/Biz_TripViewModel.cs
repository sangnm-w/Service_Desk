using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.Models
{
    public class Biz_TripViewModel
    {
        public string EMPNO { get; set; }
        public string NAME { get; set; }
        public string DEPT { get; set; }
        public string DESCRIPTION { get; set; }
        public string VEHICLE { get; set; }
        public string CONTACT_COMPANY { get; set; }
        public string CONTACT_PERSON { get; set; }
        public string CONTACT_DEPT { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public Nullable<System.DateTime> FROM_DATE { get; set; }
        public Nullable<System.DateTime> TO_DATE { get; set; }
        public Nullable<bool> USED_EQUIPMENT { get; set; }
        public string REMARK { get; set; }
        public string PLANT { get; set; }
        public byte[] DEPT_CONFIRM_IMAGE { get; set; }
        public byte[] HR_CONFIRM_IMAGE { get; set; }
        public string POSITION { get; set; }
        public string DEPTNAME { get; set; }
    }
}