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
    
    public partial class IncidentsView
    {
        internal Incident incident;

        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public string StatusId { get; set; }
        public string LevelId { get; set; }
        public string Reply { get; set; }
        public string User_create { get; set; }
        public string User_resolve { get; set; }
        public string Note { get; set; }
        public byte[] FileAttched { get; set; }
        public string FileName { get; set; }
        public Nullable<bool> Del { get; set; }
        public string DepartmentId { get; set; }
        public Nullable<bool> Solved { get; set; }
        public Nullable<System.DateTime> Solve_datetime { get; set; }
        public string Plant { get; set; }
        public string EmployeeName { get; set; }
        public string LevelName { get; set; }
        public string StatusName { get; set; }
        public string DepartmentName { get; set; }
    }
}