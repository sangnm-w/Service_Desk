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
    
    public partial class GetLastAllocationOfDevice_Result
    {
        public System.Guid Allocation_Id { get; set; }
        public Nullable<System.Guid> Device_Id { get; set; }
        public Nullable<System.Guid> Memo_Id { get; set; }
        public string Allocation_Code { get; set; }
        public string Deliver { get; set; }
        public string Receiver { get; set; }
        public Nullable<System.DateTime> Delivery_Date { get; set; }
        public Nullable<System.DateTime> Return_Date { get; set; }
        public string Department_Id { get; set; }
        public string Plant_Id { get; set; }
        public string Note { get; set; }
        public string IP { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Flag_ReAllocation { get; set; }
        public string QRCodeFile { get; set; }
    }
}
