using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Web_IT_HELPDESK
{
    public class DeviceMetadata
    {
        //[Required]
        [DisplayName("Device Type")]
        public Nullable<int> Device_Type_Id { get; set; }

        //[Required]
        [DisplayName("Device Name")]
        public string Device_Name { get; set; }

        //[Required]
        [DisplayName("Serial")]
        public string Serial_No { get; set; }

        //[Required]
        [DisplayName("Purchased Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Purchase_Date { get; set; }

        //[Required]
        [DisplayName("Depreciation")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Depreciation { get; set; }

        [DisplayName("Created Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public Nullable<DateTime> Create_Date { get; set; }

    }

    public class AllocationMetadata
    {
        [Required]
        [DisplayName("Provided Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Delivery_Date { get; set; }

        [DisplayName("Revoked Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<DateTime> Return_Date { get; set; }

        [DisplayName("Created Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public Nullable<DateTime> Create_Date { get; set; }

        [Required]
        [DisplayName("Provider")]
        public string Deliver { get; set; }

        [Required]
        public string Receiver { get; set; }

        [Required]
        [DisplayName("Department")]
        public string Department_Id { get; set; }
    }

    public class IncidentMetadata
    {
        [Required]
        public string Note { get; set; }

        [DisplayName("Created Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public Nullable<System.DateTime> datetime { get; set; }

        [DisplayName("Solved Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> Solve_datetime { get; set; }

    }

    public class ContractMetadata
    {
        [Required(ErrorMessage = "This field can not be empty.")]
        public string CONTRACTNAME { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public string VENDOR { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public string PERIODID { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public string ADDRESS { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public string PHONE { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public string REPRESENTATION { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public Nullable<int> MONTHS { get; set; }
    }

    public class Contract_SubMetadata
    {
        [Required(ErrorMessage = "This field can not be empty.")]
        public string SUBNAME { get; set; }
        [Required(ErrorMessage = "This field can not be empty.")]
        public DateTime DATE { get; set; }

    }

    public class EmployeeMetadata
    {
        [Required]
        [Display(Name = "User Login")]
        public string Emp_CJ { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Birthday { get; set; }
    }

    public class Employee_NewMetadata
    {
        [Required]
        [Display(Name = "User Login")]
        public string Emp_CJ { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Birthday { get; set; }
    }
    public class Order_Metadata
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<System.DateTime> OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<decimal> Total { get; set; }

        [Required(ErrorMessage = "Note to finish process")]
        public string Note { get; set; }

        [ScaffoldColumn(false)]
        public Nullable<bool> Confirmed { get; set; }

        [ScaffoldColumn(false)]
        public string EmployeeID { get; set; }
    }

    public class Seal_UsingMetadata
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Date { get; set; }
    }

    public class BIZ_TRIPMetadata
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? FROM_DATE { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? TO_DATE { get; set; }
    }
}