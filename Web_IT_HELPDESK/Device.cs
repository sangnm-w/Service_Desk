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
    
    public partial class Device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Device()
        {
            this.Allocations = new HashSet<Allocation>();
    
            OnConstructorInit();
        }
        partial void OnConstructorInit();
    
        public System.Guid Device_Id { get; set; }
        public Nullable<System.Guid> Contract_Id { get; set; }
        public Nullable<int> Device_Type_Id { get; set; }
        public string Device_Code { get; set; }
        public string Device_Name { get; set; }
        public string Serial_No { get; set; }
        public Nullable<System.DateTime> Purchase_Date { get; set; }
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string DISK { get; set; }
        public string Operation_System { get; set; }
        public string OS_License { get; set; }
        public string Office { get; set; }
        public string Office_License { get; set; }
        public Nullable<System.DateTime> Depreciation { get; set; }
        public string Device_Status { get; set; }
        public string Addition_Information { get; set; }
        public string Plant_Id { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Computer_Name { get; set; }
        public string Note { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Allocation> Allocations { get; set; }
        public virtual CONTRACT CONTRACT { get; set; }
        public virtual Device_Type Device_Type { get; set; }
    }
}