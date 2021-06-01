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
    
    public partial class Order_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order_()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            OnConstructorInit();
        }
        partial void OnConstructorInit();
    
        public int OrderId { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string Note { get; set; }
        public Nullable<bool> Confirmed { get; set; }
        public string EmployeeID { get; set; }
        public string Employee_Name { get; set; }
        public Nullable<bool> Del { get; set; }
        public string Plant { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
