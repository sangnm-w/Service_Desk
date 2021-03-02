using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public static class ContractHelper
    {
        public static string SaveContractFile(CONTRACT contract, HttpPostedFileBase file)
        {
            string contractPath;

            //if (!file.ContentType.StartsWith("image/"))
            //{
            //    throw new InvalidOperationException("Invalid MIME content type.");
            //}

            //Check extension of uploaded file
            var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp" };
            if (!extensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file extension.");
            }

            //Maximum allowed file size is 8 MB
            const int megabyte = 1024 * 1024;
            if (file.ContentLength > (8 * megabyte))
            {
                throw new InvalidOperationException("File size limit exceeded.");
            }

            string filePath = Path.Combine(Resources.ContractPath, contract.PLANT);
            string serverPath = HttpContext.Current.Server.MapPath(filePath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            string fileName = contract.CONTRACTNAME+ "-" + contract.DATE?.ToString("dd-MM-yyyy") + extension;
            string savePath = Path.Combine(serverPath, fileName);
            try
            {
                file.SaveAs(savePath);
                contractPath = Path.Combine(filePath, fileName);
            }
            catch (Exception)
            {
                contractPath = null;
            }

            return contractPath;
        }
    }
}