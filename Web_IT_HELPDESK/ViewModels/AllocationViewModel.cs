using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_IT_HELPDESK.ViewModels
{
    public class AllocationViewModel
    {
        public Allocation Allocation { get; set; }
        public Device Device { get; set; }

        public string Deliver_Name { get; set; }
        public string Receiver_Name { get; set; }
    }
}