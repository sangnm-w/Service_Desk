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
    
    public partial class Document
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public byte[] FileContext { get; set; }
        public string EmployeeID { get; set; }
        public string FileName { get; set; }
        public string DocumentTypeId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<bool> Del { get; set; }
    
        public virtual DocumentType DocumentType { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
