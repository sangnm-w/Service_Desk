using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Models
{
    public class AllocationModel
    {
        private AllocationModel() { }

        private static AllocationModel _instance;

        public static AllocationModel Instance { get { if (_instance == null) _instance = new AllocationModel(); return _instance; } private set => _instance = value; }

        public List<AllocationViewModel> GetLastAllocationOfDevice()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            var allocations = en.GetLastAllocationOfDevice()
                .Join(en.Employees,
                a => a.Deliver,
                e => e.EmployeeID,
                (a, e) => new
                {
                    allocation = a,
                    DeliverName = e.EmployeeName
                })
                .Join(en.Employees,
                grp => grp.allocation.Receiver,
                e => e.EmployeeID,
                (grp, e) => new
                {
                    allocation = grp.allocation,
                    DeliverName = grp.DeliverName,
                    ReceiverName = e.EmployeeName
                });

            var avm = en.Devices
                                .GroupJoin
                                (
                                    allocations,
                                    d => (Guid?)(d.Device_Id),
                                    a => a.allocation.Device_Id,
                                    (d, joined) =>
                                        new
                                        {
                                            d = d,
                                            joined = joined
                                        }
                                )
                                .SelectMany
                                (
                                    temp0 => temp0.joined.DefaultIfEmpty(),
                                    (temp0, j) =>
                                        new AllocationViewModel
                                        {
                                            Device = temp0.d,
                                            Allocation = j.allocation,
                                            Deliver_Name = j.DeliverName,
                                            Receiver_Name = j.ReceiverName
                                        }
                                );

            return avm.ToList();
        }
        public List<AllocationViewModel> GetLastAllocationOfDeviceByPlantId(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            var allocations = en.GetLastAllocationOfDevice()
                .Join(en.Employees,
                a => a.Deliver,
                e => e.EmployeeID,
                (a, e) => new
                {
                    allocation = a,
                    DeliverName = e.EmployeeName
                })
                .Join(en.Employees,
                grp => grp.allocation.Receiver,
                e => e.EmployeeID,
                (grp, e) => new
                {
                    allocation = grp.allocation,
                    DeliverName = grp.DeliverName,
                    ReceiverName = e.EmployeeName
                });

            var devices = en.Devices.Where(d => d.Plant_Id == plantId);

            var avm = devices
                                .GroupJoin
                                (
                                    allocations,
                                    d => (Guid?)(d.Device_Id),
                                    a => a.allocation.Device_Id,
                                    (d, joined) =>
                                        new
                                        {
                                            d = d,
                                            joined = joined
                                        }
                                )
                                .SelectMany
                                (
                                    temp0 => temp0.joined.DefaultIfEmpty(),
                                    (temp0, j) =>
                                        new AllocationViewModel
                                        {
                                            Device = temp0.d,
                                            Allocation = j.allocation,
                                            Deliver_Name = j.DeliverName,
                                            Receiver_Name = j.ReceiverName
                                        }
                                );

            return avm.ToList();
        }

        public List<AllocationViewModel> get_AllocationsByDeviceId(Guid? deviceId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            var allocations = en.Allocations
                .Join(en.Employees,
                a => a.Deliver,
                e => e.EmployeeID,
                (a, e) => new
                {
                    Allocation = a,
                    Deliver_Name = e.EmployeeName
                })
                .Join(en.Employees,
                j1 => j1.Allocation.Receiver,
                e => e.EmployeeID,
                (j1, e) => new AllocationViewModel
                {
                    Allocation = j1.Allocation,
                    Deliver_Name = j1.Deliver_Name,
                    Receiver_Name = e.EmployeeName
                })
                .Where(avm => avm.Allocation.Device_Id == deviceId);

            return allocations.ToList();
        }

        public string Generate_AllocationCode(Guid deviceId, string deviceCode)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<string> strAllocationCodes = en.Allocations.Where(a => a.Device_Id == deviceId).Select(a => a.Allocation_Code).ToList();

            if (strAllocationCodes.Count <= 0)
            {
                return deviceCode + "-001";
            }

            List<int> intAllocationCodes = new List<int>();
            for (int i = 0; i < strAllocationCodes.Count(); i++)
            {
                if (strAllocationCodes[i] != null)
                {
                    int length = strAllocationCodes[i].Length;
                    int code = Convert.ToInt32(strAllocationCodes[i].Substring(length - 3, 3));
                    intAllocationCodes.Add(code);
                }
            }

            int intAllocationCode = intAllocationCodes.Max() + 1;

            return deviceCode + "-" + intAllocationCode.ToString("D3");
        }
    }
}