using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Collections.Generic;
using System.IO;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class ExcelHelper
    {
        public Stream CreateExcelFile<T>(Stream stream, List<T> models, Dictionary<int, string> modelTitles, List<int> colsDate)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "Nguyen Minh Sang";
                excelPackage.Workbook.Properties.Title = "IT Service Desk";
                excelPackage.Workbook.Properties.Comments = "";
                excelPackage.Workbook.Worksheets.Add("First Sheet");

                //Format Sheet
                var workSheet = excelPackage.Workbook.Worksheets[0];
                //workSheet.DefaultColWidth = 10;
                workSheet.Cells[1, 1].LoadFromCollection(models, true, TableStyles.Medium7);


                // TODO: Upgrade - Use list: list<string> modelTtiles foreach with i++
                foreach (int i in modelTitles.Keys)
                {
                    workSheet.Cells[1, i].Value = modelTitles[i];
                }

                foreach (int colnum in colsDate)
                {
                    workSheet.Cells[2, colnum, models.Count + 1, colnum].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
                }

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