using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Web_IT_HELPDESK.Properties;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public static class ContractHelper
    {
        public static string SaveContractFile(HttpPostedFileBase file, string filePath, string fileName)
        {
            string result = null;
            //if (!file.ContentType.StartsWith("image/"))
            //{
            //    throw new InvalidOperationException("Invalid MIME content type.");
            //}

            //Check extension of uploaded file
            //var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            //string[] extensions = { ".gif", ".jpg", ".png", ".svg", ".webp" };
            //if (!extensions.Contains(extension))
            //{
            //    throw new InvalidOperationException("Invalid file extension.");
            //}

            //Maximum allowed file size is 8 MB
            const int megabyte = 1024 * 1024;
            if (file.ContentLength > (8 * megabyte))
            {
                throw new InvalidOperationException("File size limit exceeded.");
            }

            string serverPath = HostingEnvironment.MapPath(filePath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            string savePath = Path.Combine(serverPath, fileName);

            try
            {
                file.SaveAs(savePath);
                result = Path.Combine(filePath, fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static string SaveContractFile(CONTRACT contract, HttpPostedFileBase file)
        {
            string filePath = Path.Combine(Resources.ContractPath, contract.PLANT);

            var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            string fileName = contract.ID + "_" + contract.DATE?.ToString("dd-MM-yyyy") + extension;

            string savePath = SaveContractFile(file, filePath, fileName);
            return savePath;
        }
        public static string SaveContractFile(CONTRACT_SUB contractSub, HttpPostedFileBase file)
        {
            string filePath = Path.Combine(Resources.ContractPath, contractSub.PLANT);

            var extension = Path.GetExtension(file.FileName.ToLowerInvariant());
            string fileName = contractSub.ID + "_" + contractSub.DATE?.ToString("dd-MM-yyyy") + extension;

            string savePath = SaveContractFile(file, filePath, fileName);
            return savePath;
        }
    }
}