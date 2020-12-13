using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Web_IT_HELPDESK.Models;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class SealUsingHelper
    {
        private static SealUsingHelper _Instance;

        public static SealUsingHelper Instance
        {
            get { if (_Instance == null) _Instance = new SealUsingHelper(); return _Instance; }
            set => _Instance = value;
        }
        private SealUsingHelper() { }

        public bool sendSealUsingEmail(Seal_Using sealUsing, int level_confirm, Employee userRequest)
        {
            string subject = "";
            string body = "";
            Content_Email(sealUsing, level_confirm, out subject, out body);
            return InformationHelper.Instance.Send_Mail(level_confirm, subject, body, userRequest);
        }

        private void Content_Email(Seal_Using sealUsing, int level, out string subject, out string body)
        {
            string domainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            subject = "";
            body = "";
            string departmentName = DepartmentModel.Instance.getDeptName(sealUsing.Plant, sealUsing.DepartmentId);
            if (level == 1) // Level 1: Department Manager
            {
                subject = "[Cần Duyệt] - Phiếu yêu cầu sử dụng con dấu: " + sealUsing.Employee_name + " - tạo ngày: " + sealUsing.Date;

                body = "Tên người yêu cầu: " + sealUsing.Employee_name + "\n" +
                        "          Bộ phận: " + departmentName + "\n" +
                        "     Ngày yêu cầu: " + sealUsing.Date + "\n" +
                        "  Ngày ký văn bản: " + sealUsing.Date_signature + "\n" +
                        "     Loại văn bản: " + sealUsing.Type_document + "\n" +
                        " Nội dung văn bản: " + sealUsing.Context + "\n" +
                        "         Nơi nhận: " + sealUsing.Place_Recipient + "\n" +
                        " Người ký văn bản: " + sealUsing.Name_signature + "\n" +
                        "   Theo đường dẫn: " + domainName + "/servicedesk/SealUsing/Edit/" + sealUsing.Id + "\n" + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n";

            }
            else if (level == 2) // Level 2: Resend
            {
                subject = "<<Gấp>> [Cần duyệt] - Phiếu yêu cầu sử dụng con dấu: " + sealUsing.Employee_name + " - tạo ngày: " + sealUsing.Date;
                body = "Tên người yêu cầu: " + sealUsing.Employee_name + "\n" +
                        "          Bộ phận: " + departmentName + "\n" +
                        "     Ngày yêu cầu: " + sealUsing.Date + "\n" +
                        "  Ngày ký văn bản: " + sealUsing.Date_signature + "\n" +
                        "     Loại văn bản: " + sealUsing.Type_document + "\n" +
                        " Nội dung văn bản: " + sealUsing.Context + "\n" +
                        "         Nơi nhận: " + sealUsing.Place_Recipient + "\n" +
                        " Người ký văn bản: " + sealUsing.Name_signature + "\n" +
                        "   Theo đường dẫn: " + domainName + "/servicedesk/SealUsing/Edit/" + sealUsing.Id + "\n" + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n";
            }
            else if (level == 5) // Level 5: HR Admin
            {
                subject = "[Cần Duyệt] - Phiếu yêu cầu sử dụng con dấu: " + sealUsing.Employee_name + " - tạo ngày: " + sealUsing.Date;
                body = "Tên người yêu cầu: " + sealUsing.Employee_name + "\n" +
                        "          Bộ phận: " + departmentName + "\n" +
                        "     Ngày yêu cầu: " + sealUsing.Date + "\n" +
                        "  Ngày ký văn bản: " + sealUsing.Date_signature + "\n" +
                        "     Loại văn bản: " + sealUsing.Type_document + "\n" +
                        " Nội dung văn bản: " + sealUsing.Context + "\n" +
                        "         Nơi nhận: " + sealUsing.Place_Recipient + "\n" +
                        " Người ký văn bản: " + sealUsing.Name_signature + "\n" +
                        "-------------------------------------" + "\n" +
                        "Đã được trưởng phòng duyệt" + "\n" +
                        "   Theo đường dẫn: " + domainName + "/servicedesk/SealUsing/Confirm/" + sealUsing.Id + "\n" +
                        "Trân trọng!" + "\n" + "\n" + "\n";
            }
            else if (level == 6)
            {
                subject = "[Không được duyệt] - Phiếu yêu cầu sử dụng con dấu: " + sealUsing.Employee_name + " - tạo ngày: " + sealUsing.Date;
                body = "Tên người yêu cầu: " + sealUsing.Employee_name + "\n" +
                   "          Bộ phận: " + departmentName + "\n" +
                   "     Ngày yêu cầu: " + sealUsing.Date + "\n" +
                   "  Ngày ký văn bản: " + sealUsing.Date_signature + "\n" +
                   "     Loại văn bản: " + sealUsing.Type_document + "\n" +
                   " Nội dung văn bản: " + sealUsing.Context + "\n" +
                   "         Nơi nhận: " + sealUsing.Place_Recipient + "\n" +
                   " Người ký văn bản: " + sealUsing.Name_signature + "\n" +
                   "Trân trọng!" + "\n" + "\n" + "\n";
            }
            else if (level == 7)
            {
                subject = "[Đã được duyệt] - Phiếu yêu cầu sử dụng con dấu: " + sealUsing.Employee_name + " - tạo ngày: " + sealUsing.Date;
                body = "Tên người yêu cầu: " + sealUsing.Employee_name + "\n" +
                   "          Bộ phận: " + departmentName + "\n" +
                   "     Ngày yêu cầu: " + sealUsing.Date + "\n" +
                   "  Ngày ký văn bản: " + sealUsing.Date_signature + "\n" +
                   "     Loại văn bản: " + sealUsing.Type_document + "\n" +
                   " Nội dung văn bản: " + sealUsing.Context + "\n" +
                   "         Nơi nhận: " + sealUsing.Place_Recipient + "\n" +
                   " Người ký văn bản: " + sealUsing.Name_signature + "\n" +
                   "Trân trọng!" + "\n" + "\n" + "\n";
            }
        }
    }
}