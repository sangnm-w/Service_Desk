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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.BIZ_TRIP = new HashSet<BIZ_TRIP>();
            this.CONTRACTs = new HashSet<CONTRACT>();
            this.Documents = new HashSet<Document>();
            this.EMP_ANSWER = new HashSet<EMP_ANSWER>();
            this.Order_ = new HashSet<Order_>();
            this.Rights_Management = new HashSet<Rights_Management>();
            this.UserLogons = new HashSet<UserLogon>();
            this.Employee_Screen = new HashSet<Employee_Screen>();
            OnConstructorInit();
        }
        partial void OnConstructorInit();
    
        public string Emp_CJ { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string ID_Number { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Marital_Status { get; set; }
        public string Position { get; set; }
        public string Job { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Hire_Date { get; set; }
        public Nullable<System.DateTime> Termination_Date { get; set; }
        public string Department_Id { get; set; }
        public string Plant_Id { get; set; }
        public Nullable<bool> Deactive { get; set; }
        public string Password { get; set; }
        public Nullable<bool> Administrator { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BIZ_TRIP> BIZ_TRIP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRACT> CONTRACTs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Document> Documents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMP_ANSWER> EMP_ANSWER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_> Order_ { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rights_Management> Rights_Management { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLogon> UserLogons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee_Screen> Employee_Screen { get; set; }
    }
}
