using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Models
{
    public class IncidentModel
    {
        private IncidentModel() { }

        private static IncidentModel _instance;

        public static IncidentModel Instance { get { if (_instance == null) _instance = new IncidentModel(); return _instance; } private set => _instance = value; }

        public IEnumerable<IncidentViewModel> get_IEnum_Inc()
        {
            ServiceDeskEntities en = new ServiceDeskEntities();
            var incMode = en.Incidents
                .Join(
                    en.Status,
                    inc => inc.StatusId,
                    s => s.StatusId,
                    (inc, s) => new { inc, s.StatusName }
                )
                .Join(
                    en.Levels,
                    iws => iws.inc.LevelId,
                    lv => lv.LevelId,
                    (iws, lv) => new { iws.inc, iws.StatusName, lv.LevelName }
                )
                .Join(
                    en.Departments,
                    iwsal => iwsal.inc.DepartmentId,
                    dept => dept.Department_ID,
                    (iwsal, dept) => new { iwsal.inc, iwsal.StatusName, iwsal.LevelName, dept.Department_Name }
                )
                .Join(
                    //en.Employees,
                    en.Employee_New,
                    isld => isld.inc.User_create,
                    e => e.Emp_CJ,
                    (isld, e) => new { isld.inc, isld.StatusName, isld.LevelName, isld.Department_Name, e.Employee_Name }
                )
                .GroupJoin(
                    //en.Employees,
                    en.Employee_New,
                    i => i.inc.User_resolve,
                    e => e.Emp_CJ,
                    (i, employeesGroup) => new { i, employeesGroup }
                )
                .SelectMany(
                    temp0 => temp0.employeesGroup.DefaultIfEmpty(),
                    (temp0, emp) => new IncidentViewModel
                    {
                        Code = temp0.i.inc.Code,
                        datetime = temp0.i.inc.datetime,
                        Del = temp0.i.inc.Del,
                        FileAttched = temp0.i.inc.FileAttched,
                        FileName = temp0.i.inc.FileName,
                        Id = temp0.i.inc.Id,
                        Note = temp0.i.inc.Note,
                        Plant = temp0.i.inc.Plant,
                        Reply = temp0.i.inc.Reply,
                        Solved = temp0.i.inc.Solved,
                        Solve_datetime = temp0.i.inc.Solve_datetime,
                        User_create = temp0.i.inc.User_create,
                        User_resolve = temp0.i.inc.User_resolve,
                        StatusId = temp0.i.inc.StatusId,
                        LevelId = temp0.i.inc.LevelId,
                        DepartmentId = temp0.i.inc.DepartmentId,
                        userCreateName = temp0.i.Employee_Name,
                        userResolveName = emp.Employee_Name,
                        statusName = temp0.i.StatusName,
                        levelName = temp0.i.LevelName,
                        departmentName = temp0.i.Department_Name
                    }
                )
                .Distinct();

            return incMode;
        }

        public IncidentViewModel get_Incident(Guid incID)
        {
            return Instance.get_IEnum_Inc().FirstOrDefault(i => i.Id == incID);
        }

        public string Generate_IncidentCode(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<string> strIncidentCodes = en.Incidents.Where(i => i.Code != null && i.Plant == plantId).Select(i => i.Code).ToList();

            if (strIncidentCodes.Count <= 0)
            {
                return plantId + "-00001";
            }

            List<int> intIncidentCodes = new List<int>();
            for (int i = 0; i < strIncidentCodes.Count(); i++)
            {
                if (strIncidentCodes[i] != null)
                {
                    int length = strIncidentCodes[i].Length;
                    int code = Convert.ToInt32(strIncidentCodes[i].Substring(length - 5, 5));
                    intIncidentCodes.Add(code);
                }
            }

            int intIncidentCode = intIncidentCodes.Max() + 1;
            string strIncidentCode = intIncidentCode.ToString("D5");

            return plantId + "-" + strIncidentCode;
        }

        public List<string> GetITMemberEmails(string plantId)
        {
            ServiceDeskEntities en = new ServiceDeskEntities();

            List<string> result = new List<string>();
            if (plantId == "V2010")
            {
                string departmentId = "V2090S0001";
                result = en.Employee_New
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                    .Where(grp => grp.d.Plant_ID == "V2090" && grp.d.Department_ID == departmentId && grp.e.Deactive != true)
                    .Select(grp => grp.e.Email).ToList();
            }
            else if (plantId == "V2080")
            {
                string departmentId = "V2020S0001";
                result = en.Employee_New
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                    .Where(grp => grp.d.Plant_ID == "V2020" && grp.d.Department_ID == departmentId && grp.e.Deactive != true)
                    .Select(grp => grp.e.Email).ToList();
            }
            else
            {
                string departmentId = plantId + "S0001";
                result = en.Employee_New
                    .Join(en.Departments, e => e.Department_ID, d => d.Department_ID, (e, d) => new { e, d })
                    .Where(grp => grp.d.Plant_ID == plantId && grp.d.Department_ID == departmentId && grp.e.Deactive != true)
                    .Select(grp => grp.e.Email).ToList();
            }

            return result;
        }
    }
}