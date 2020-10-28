using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK
{
    [MetadataType(typeof(DeviceMetadata))]
    public partial class Device { }

    [MetadataType(typeof(AllocationMetadata))]
    public partial class Allocation { }

    [MetadataType(typeof(IncidentMetadata))]
    public partial class Incident
    {
        public Incident()
        {
            Id = Guid.NewGuid();
            datetime = DateTime.Now;
            Solved = false;
            Del = false;
        }
    }

    public partial class BIZ_TRIP
    {
        public BIZ_TRIP()
        {
            DEPT_CONFIRM = false;
            HR_CONFIRM = false;
            DEL = false;
            USED_EQUIPMENT = false;
            DATE = DateTime.Now;
            FROM_DATE = DateTime.Now;
            TO_DATE = DateTime.Now;
            ID = Guid.NewGuid();
            IT_CONFIRM = false;
        }
    }

    [MetadataType(typeof(ContractMetadata))]
    public partial class CONTRACT { }

    [MetadataType(typeof(Contract_SubMetadata))]
    public partial class CONTRACT_SUB { }

    public partial class Document
    {
        public Document()
        {
            Id = Guid.NewGuid();
            Code = create_code();
            Date = DateTime.Now;
            Del = false;
        }

        public static string create_code()
        {
            try
            {
                ServiceDeskEntities en = new ServiceDeskEntities();
                int ct = (from i in en.Documents select i).Count();
                ct = ct + 1;
                string soct = ct.ToString("000000");
                return soct;
            }
            catch
            {
                //TODO: do something when cannot generate so chung tu
                return "0000001";
            }
        }
    }

    public partial class Drinking_Request
    {
        public Drinking_Request()
        {
            Date = DateTime.Now;
            Department_confirm = false;
            HR_confirm = false;
            Del = false;
        }
    }

    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee { }

    [MetadataType(typeof(Order_Metadata))]
    public partial class Order_ {
        partial void OnConstructorInit()
        {
            Confirmed = false;
            OrderDate = DateTime.Now;
        }
    }

    public partial class Seal_Using
    {
        public Seal_Using()
        {
            Date = DateTime.Now;
            Period_date = DateTime.Now;
            Department_confirm = false;
            Employee_Seal_keep_confrim = false;
            Date_signature = DateTime.Now;
            Del = false;
        }
    }
}