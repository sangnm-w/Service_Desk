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
    
    public partial class Rule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rule()
        {
            this.Rights_Management = new HashSet<Rights_Management>();
            OnConstructorInit();
        }
        partial void OnConstructorInit();
    
        public int Id { get; set; }
        public string Rule_Name { get; set; }
        public Nullable<bool> Deactive { get; set; }
        public string Module_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rights_Management> Rights_Management { get; set; }
    }
}
