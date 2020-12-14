using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class Biz_TripHelper
    {
        private static Biz_TripHelper _Instance;

        public static Biz_TripHelper Instance
        {
            get { if (_Instance == null) _Instance = new Biz_TripHelper(); return _Instance; }
            set => _Instance = value;
        }
        private Biz_TripHelper() { }

        public bool sendBiz_TripEmail(BIZ_TRIP biztrip, int level_confirm, Employee userRequest, string linkConfirm = null)
        {
            string subject = "";
            string body = "";
            Content_Email(biztrip, level_confirm, linkConfirm, out subject, out body);
            return InformationHelper.Instance.Send_Mail(level_confirm, subject, body, userRequest);
        }

        private void Content_Email(BIZ_TRIP biztrip, int level, string linkConfirm, out string subject, out string body)
        {
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            subject = "";
            body = "";
            string departmentName = DepartmentModel.Instance.getDeptName(biztrip.PLANT, biztrip.DEPT);
            if (level == 1) // Level 1: Department Manager
            {
                subject = "[Need Confirm by Department Manager ] - Phiếu đăng ký đi công tác: " + biztrip.NAME + " - tạo ngày: " + biztrip.DATE;

                body = "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "   Follow link to confirm: " + linkConfirm + biztrip.ID + "\n" + "\n" +
                   "Regards!";

            }
            else if (level == 2) // Level 2: Resend
            {
                subject = "<<Gấp>> [Need Confirm by Department Manager ] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - tạo ngày: " + biztrip.DATE;
                body = "     Employee Name: " + biztrip.NAME + "\n" +
                      "        Department: " + departmentName + "\n" +
                      "              Date: " + biztrip.DATE + "\n" +
                      "       Description: " + biztrip.DESCRIPTION + "\n" +
                      "           Vehicle: " + biztrip.VEHICLE + "\n" +
                      "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                      "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                      "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                      "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                      "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                      " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                      "-------------------------------------" + "\n" +
                      "   Follow link to confirm: " + domainName + "/servicedesk/BIZ_TRIP/dept_confirm/" + biztrip.ID + "\n" + "\n" +
                      "Regards!";
            } // TODO: Need Action Method
            else if (level == 3) // Level 3: BOD
            {
                subject = "[Need Confirm by BOD] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - ngày: " + biztrip.DATE;
                body =
                   "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "Confirmed by Head of Department" + "\n" +
                   "   Follow to confirm by link: " + domainName + "/servicedesk/BIZ_TRIP/bod_confirm/" + biztrip.ID + "\n" +
                   "Regards!";

            }
            else if (level == 4) // Level 4: HR Manager
            {
                subject = "[Need Confirm by HR Manager] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - ngày: " + biztrip.DATE;
                body =
                   "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "Confirmed by Head of Department" + "\n" +
                   "   Follow to confirm by link: " + domainName + "/servicedesk/BIZ_TRIP/hr_confirm/" + biztrip.ID + "\n" +
                   "Regards!";

            }// TODO: Need Action Method
            else if (level == 5) // Level 5: HR Admin
            {
                subject = "[Need Confirm by HR Admin] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - ngày: " + biztrip.DATE;
                body =
                   "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "Confirmed" + "\n" +
                   "   Follow to add information by link: " + domainName + "/servicedesk/BIZ_TRIP/hr_admin/" + biztrip.ID + "\n" +
                   "Regards!";

            }
            else if (level == 6)
            {
                subject = "[NOT APPROVED] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - ngày: " + biztrip.DATE;
                body =
                   "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "Regards!";
            }
            else if (level == 7)
            {
                subject = "[APPROVED] - Phiếu yêu cầu đăng ký đi công tác BIZ TRIP: " + biztrip.NAME + " - ngày: " + biztrip.DATE;
                body =
                   "     Employee Name: " + biztrip.NAME + "\n" +
                   "        Department: " + departmentName + "\n" +
                   "              Date: " + biztrip.DATE + "\n" +
                   "       Description: " + biztrip.DESCRIPTION + "\n" +
                   "           Vehicle: " + biztrip.VEHICLE + "\n" +
                   "  Conctact Company: " + biztrip.CONTACT_COMPANY + "\n" +
                   "Conctact Departent: " + biztrip.CONTACT_DEPT + "\n" +
                   "    Contact person: " + biztrip.CONTACT_PERSON + "\n" +
                   "         From date: " + biztrip.FROM_DATE + "       To date: " + biztrip.TO_DATE + "\n" +
                   "    Used equipemnt: " + biztrip.USED_EQUIPMENT.ToString() + "\n" +
                   " Equipemnt remarks: " + biztrip.REMARK.ToString() + "\n" +
                   "-------------------------------------" + "\n" +
                   "Regards!";
            }
        }
    }
}