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
    
    public partial class EMP_ANSWER
    {
        public System.Guid ID { get; set; }
        public Nullable<System.Guid> QUESTION_ID { get; set; }
        public string EMPLOYEEID { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public Nullable<int> ANSWERID { get; set; }
        public string NOTE { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual QUESTION QUESTION { get; set; }
    }
}
