using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.ViewModels
{
    public class SealUsingViewModel
    {
        public class IndexSealUsing
        {
            public Seal_Using SealUsing { get; set; }
            public string DeptName { get; set; }
            public string PlantName { get; set; }
        }
        public class CreateSealUsing : Seal_Using
        {
            public CreateSealUsing()
            {
                Date = DateTime.Now;
                Period_date = DateTime.Now;
                Employee_Seal_keep_confrim = false;
                Date_signature = DateTime.Now;
                Del = false;

                Employee_ID = CurrentUser.Instance.User.Emp_CJ;
                Employee_name = CurrentUser.Instance.User.EmployeeName;
                Plant = CurrentUser.Instance.User.Plant_Id;
                DepartmentId = CurrentUser.Instance.User.Department_Id;
                DeptName = DepartmentModel.Instance.getDeptName(Plant, DepartmentId);
            }

            public Seal_Using CreateSealUsing_To_SealUsing(Seal_Using su)
            {
                su.Date = Date;
                su.DepartmentId = DepartmentId;
                su.Employee_name = Employee_name;
                su.Employee_ID = Employee_ID;
                su.Code_number = Code_number;
                su.Type_document = Type_document;
                su.Name_signature = Name_signature;
                su.Date_signature = Date_signature;
                su.Context = Context;
                su.Place_Recipient = Place_Recipient;
                su.Plant = Plant;
                return su;
            }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new string Type_document { get; set; }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new string Code_number { get; set; }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new string Context { get; set; }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new string Place_Recipient { get; set; }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new DateTime? Date_signature { get; set; }
            [Required(ErrorMessage = "This field can not be empty.")]
            public new string Name_signature { get; set; }
            public string DeptName { get; set; }
            public string PlantName { get; set; }
        }

        public class EditSealUsing : Seal_Using
        {
            public EditSealUsing() { }
            public EditSealUsing(Seal_Using su)
            {
                Id = su.Id;
                Date = su.Date;
                Employee_ID = su.Employee_ID;
                Employee_name = su.Employee_name;
                Type_document = su.Type_document;
                DepartmentId = su.DepartmentId;
                Code_number = su.Code_number;
                Context = su.Context;
                Place_Recipient = su.Place_Recipient;
                Period_date = su.Period_date;
                Employee_Seal_keep = su.Employee_Seal_keep;
                Department_confirm = su.Department_confirm;
                Employee_Seal_keep_confrim = su.Employee_Seal_keep_confrim;
                Del = su.Del;
                Department_note = su.Department_note;
                Department_confirm_date = su.Department_confirm_date;
                Employee_Seal_keep_note = su.Employee_Seal_keep_note;
                Date_signature = su.Date_signature;
                Name_signature = su.Name_signature;
                Plant = su.Plant;
            }
            public Seal_Using EditSealUsing_To_SealUsing(Seal_Using su)
            {
                su.Department_confirm = Department_confirm;
                su.Department_note = Department_note;
                su.Department_confirm_date = Department_confirm_date;

                return su;
            }
            public new bool Department_confirm { get; set; }
            public new string Department_note { get; set; }
            public new DateTime? Department_confirm_date { get; set; }
            public string DeptName { get; set; }
            public string PlantName { get; set; }
        }

        public class ConfirmSealUsing : Seal_Using
        {
            public ConfirmSealUsing() { }
            public ConfirmSealUsing(Seal_Using su)
            {
                Id = su.Id;
                Date = su.Date;
                Employee_ID = su.Employee_ID;
                Employee_name = su.Employee_name;
                Type_document = su.Type_document;
                DepartmentId = su.DepartmentId;
                Code_number = su.Code_number;
                Context = su.Context;
                Place_Recipient = su.Place_Recipient;
                Period_date = su.Period_date;
                Employee_Seal_keep = su.Employee_Seal_keep;
                Department_confirm = su.Department_confirm;
                Employee_Seal_keep_confrim = su.Employee_Seal_keep_confrim;
                Del = su.Del;
                Department_note = su.Department_note;
                Department_confirm_date = su.Department_confirm_date;
                Employee_Seal_keep_note = su.Employee_Seal_keep_note;
                Date_signature = su.Date_signature;
                Name_signature = su.Name_signature;
                Plant = su.Plant;
            }
            public Seal_Using ConfirmSealUsing_To_SealUsing(Seal_Using su)
            {
                su.Employee_Seal_keep = Employee_Seal_keep;
                su.Employee_Seal_keep_confrim = Employee_Seal_keep_confrim;
                su.Employee_Seal_keep_note = Employee_Seal_keep_note;

                return su;
            }
            public new DateTime? Employee_Seal_keep { get; set; }
            public new bool Employee_Seal_keep_confrim { get; set; }
            public new string Employee_Seal_keep_note { get; set; }
            public string DeptName { get; set; }
            public string PlantName { get; set; }
        }
        public class DetailsSealUsing : Seal_Using
        {
            public DetailsSealUsing() { }
            public DetailsSealUsing(Seal_Using su)
            {
                Id = su.Id;
                Date = su.Date;
                Employee_ID = su.Employee_ID;
                Employee_name = su.Employee_name;
                Type_document = su.Type_document;
                DepartmentId = su.DepartmentId;
                Code_number = su.Code_number;
                Context = su.Context;
                Place_Recipient = su.Place_Recipient;
                Period_date = su.Period_date;
                Employee_Seal_keep = su.Employee_Seal_keep;
                Department_confirm = su.Department_confirm;
                Employee_Seal_keep_confrim = su.Employee_Seal_keep_confrim;
                Del = su.Del;
                Department_note = su.Department_note;
                Department_confirm_date = su.Department_confirm_date;
                Employee_Seal_keep_note = su.Employee_Seal_keep_note;
                Date_signature = su.Date_signature;
                Name_signature = su.Name_signature;
                Plant = su.Plant;
            }
            public new DateTime? Employee_Seal_keep { get; set; }
            public new bool Employee_Seal_keep_confrim { get; set; }
            public new string Employee_Seal_keep_note { get; set; }
            public string DeptName { get; set; }
            public string PlantName { get; set; }
        }
    }
}
