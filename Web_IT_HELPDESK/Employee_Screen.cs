//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_IT_HELPDESK
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee_Screen
    {
        public System.Guid Id { get; set; }
        public Nullable<int> ScreenId { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<bool> Use { get; set; }
        public Nullable<bool> Read { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Screen Screen { get; set; }
    }
}
