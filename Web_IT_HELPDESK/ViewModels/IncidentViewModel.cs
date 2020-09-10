using System;
using System.Collections.Generic;
using System.Linq;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.ViewModels
{
    public class IncidentViewModel
    {
        private IncidentViewModel() { }

        private static IncidentViewModel _instance;

        public static IncidentViewModel Instance { get { if (_instance == null) _instance = new IncidentViewModel(); return _instance; } private set => _instance = value; }

        public IEnumerable<IncidentModel> get_IEnum_Inc()
        {
            Web_IT_HELPDESKEntities en = new Web_IT_HELPDESKEntities();
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
                    iwsal => new { iwsal.inc.DepartmentId, iwsal.inc.Plant },
                    dept => new { dept.DepartmentId, dept.Plant },
                    (iwsal, dep) => new { iwsal.inc, iwsal.StatusName, iwsal.LevelName, dep.DepartmentName }
                )
                .Join(
                    en.Employees,
                    isld => isld.inc.User_create,
                    e => e.EmployeeID,
                    (isld, e) => new { isld.inc, isld.StatusName, isld.LevelName, isld.DepartmentName, e.EmployeeName }
                )
                .GroupJoin(
                    en.Employees,
                    i => i.inc.User_resolve,
                    e => e.EmployeeID,
                    (i, employeesGroup) => new { i, employeesGroup }
                )
                .SelectMany(
                    temp0 => temp0.employeesGroup.DefaultIfEmpty(),
                    (temp0, emp) => new IncidentModel
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
                        userCreateName = temp0.i.EmployeeName,
                        userResolveName = emp.EmployeeName,
                        statusName = temp0.i.StatusName,
                        levelName = temp0.i.LevelName,
                        departmentName = temp0.i.DepartmentName
                    }
                )
                .Select(all => all)
                .Distinct();

            return incMode;
        }

        public IncidentModel get_single_Inc(Guid incID)
        {
            return Instance.get_IEnum_Inc().FirstOrDefault(i => i.Id == incID);
        }
    }
}