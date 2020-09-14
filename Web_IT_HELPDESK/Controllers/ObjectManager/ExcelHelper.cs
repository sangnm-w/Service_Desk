using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Collections.Generic;
using System.IO;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class ExcelHelper
    {
        public Stream CreateExcelFile<T>(Stream stream, List<T> models)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "Nguyen Minh Sang";
                excelPackage.Workbook.Properties.Title = "IT order request";
                excelPackage.Workbook.Properties.Comments = "";
                excelPackage.Workbook.Worksheets.Add("First Sheet");

                //Format Sheet
                var workSheet = excelPackage.Workbook.Worksheets[0];
                //workSheet.DefaultColWidth = 10;
                workSheet.Cells[1, 1].LoadFromCollection(models, true, TableStyles.Medium7);

                Dictionary<int, string> incTitle = ExcelTitle.Instance.IncTitles();
                foreach (int i in incTitle.Keys)
                {
                    workSheet.Cells[1, i].Value = ExcelTitle.Instance.IncTitles()[i];
                }
                workSheet.Cells[2, 2, models.Count + 1, 2].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns(15, 45);
                workSheet.Cells.Style.WrapText = true;
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }

        private ExcelHelper() { }

        private static ExcelHelper _instance;

        public static ExcelHelper Instance { get { if (_instance == null) _instance = new ExcelHelper(); return _instance; } private set => _instance = value; }
    }
}