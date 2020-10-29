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
    
    public partial class CONTRACT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTRACT()
        {
            this.CONTRACT_SUB = new HashSet<CONTRACT_SUB>();
            this.Devices = new HashSet<Device>();
    
            OnConstructorInit();
        }
        partial void OnConstructorInit();
    
        public System.Guid ID { get; set; }
        public string VENDOR { get; set; }
        public string PERIODID { get; set; }
        public string ADREES { get; set; }
        public string PHONE { get; set; }
        public string REPRESENTATION { get; set; }
        public Nullable<System.DateTime> DATE { get; set; }
        public Nullable<System.DateTime> DATE_CREATE { get; set; }
        public string CONTRACTNAME { get; set; }
        public byte[] CONTENT { get; set; }
        public string NOTE { get; set; }
        public string USER_CREATE { get; set; }
        public string DEPARTMENTID { get; set; }
        public Nullable<bool> DEL { get; set; }
        public Nullable<int> MONTHS { get; set; }
        public string CONTRACT_TYPE_ID { get; set; }
        public Nullable<System.DateTime> DATE_MATURITY { get; set; }
        public string PLANT { get; set; }
        public string CONTRACT_TYPE_DT_ID { get; set; }
    
        public virtual CONTRACT_TYPE CONTRACT_TYPE { get; set; }
        public virtual CONTRACT_TYPE_DETAIL CONTRACT_TYPE_DETAIL { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual PERIOD PERIOD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTRACT_SUB> CONTRACT_SUB { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Device> Devices { get; set; }
    }
}
