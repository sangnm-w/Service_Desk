//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_IT_HELPDESK
{
    using System;
    using System.Collections.Generic;
    
    public partial class CJ_HR_SALARY_REPORT
    {
        public string ID_NUM { get; set; }
        public string POSITION { get; set; }
        public string PURPOSE { get; set; }
        public string DEPARTMENT { get; set; }
        public string FULLNAME { get; set; }
        public string GENDER { get; set; }
        public string DOB { get; set; }
        public string RESIDENT { get; set; }
        public string EDUCATION { get; set; }
        public string EXPERIENCE { get; set; }
        public string JOIN_DATE { get; set; }
        public string WORKING_TIME { get; set; }
        public Nullable<decimal> BASIC_SALARY { get; set; }
        public string GRADE { get; set; }
        public Nullable<decimal> MEAL { get; set; }
        public Nullable<decimal> POSITION_ALLOWANCE { get; set; }
        public Nullable<decimal> PHONE_ALLOWANCE { get; set; }
        public Nullable<decimal> PRODUCTIVITY_BONUS { get; set; }
        public string PERIOD_WORKING { get; set; }
        public string NOTE { get; set; }
        public System.Guid ID { get; set; }
        public byte[] SIGNATURE_IMAGE { get; set; }
        public Nullable<bool> HR_APPROVE { get; set; }
        public byte[] SIGNATURE_HR_IMAGE { get; set; }
        public Nullable<System.DateTime> HR_APPROVE_DATE { get; set; }
        public Nullable<bool> SALES_APPROVE { get; set; }
        public byte[] SIGNATURE_SALES_IMAGE { get; set; }
        public Nullable<System.DateTime> SALES_APPROVE_DATE { get; set; }
        public Nullable<bool> SALES_EMPLOYEE { get; set; }
        public Nullable<System.DateTime> SIGNATURE_BOD_DATE { get; set; }
    
        public virtual CJ_HR_RECRUITMENT CJ_HR_RECRUITMENT { get; set; }
    }
}
