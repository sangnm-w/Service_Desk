using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Web_IT_HELPDESK.ViewModels;

namespace Web_IT_HELPDESK.Controllers.ObjectManager
{
    public class ExcelHelper
    {
        public Stream CreateExcelFile<T>(Stream stream, List<T> model, Dictionary<int, string> modelTitles, List<int> colsDate)
        {
            Stream res = null;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Properties.Author = "Nguyen Minh Sang";
                excelPackage.Workbook.Properties.Title = "IT Service Desk";
                excelPackage.Workbook.Properties.Comments = "";
                excelPackage.Workbook.Worksheets.Add("First Sheet");

                //Format Sheet
                var workSheet = excelPackage.Workbook.Worksheets[0];
                workSheet.DefaultColWidth = 10;

                // Load model to excel (Content)
                workSheet.Cells[1, 1].LoadFromCollection(model, true, TableStyles.Medium7);
               

                //Add model titles to excel(Header)
                foreach (int i in modelTitles.Keys)
                {
                    workSheet.Cells[1, i].Value = modelTitles[i];
                }

                //Format columns need to be Datetime
                if (colsDate != null)
                {
                    foreach (int colnum in colsDate)
                    {
                        workSheet.Cells[2, colnum, model.Count + 1, colnum].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
                    }
                }

                // By default, the column width is not  
                // set to auto fit for the content 
                // of the range, so we are using 
                // AutoFitColumns(minWidth, maxWidth) and Wraptext here.
                // Because we don't want column over width when render long text.
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns(15, 45);
                workSheet.Cells.Style.WrapText = true;
                excelPackage.Save();
                res = excelPackage.Stream;
            }
            return res;
        }

        //public Stream CreateExcelFile<T>(Stream stream, List<T> model, Dictionary<int, string> modelTitles, List<int> colsDate)
        //{
        //    Stream res = null;
        //    using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
        //    {
        //        excelPackage.Workbook.Properties.Author = "Nguyen Minh Sang";
        //        excelPackage.Workbook.Properties.Title = "IT Service Desk";
        //        excelPackage.Workbook.Properties.Comments = "";
        //        excelPackage.Workbook.Worksheets.Add("First Sheet");

        //        //Format Sheet
        //        var workSheet = excelPackage.Workbook.Worksheets[0];
        //        workSheet.DefaultColWidth = 10;

        //        // Load model to excel (Content)
        //        // Option 1: load form collection
        //        workSheet.Cells[1, 1].LoadFromCollection(model, true, TableStyles.Medium7);
        //        // Option 2: Load by foreach
        //        // Inserting the data into excel 
        //        // sheet by using the for each loop 
        //        // As we have values to the first row  
        //        // we will start with second row 
        //        //int recordIndex = 2;
        //        //foreach (var m in model)
        //        //{
        //        //    workSheet.Row(recordIndex).Height = 150;
        //        //    int countProperties = m.GetType().GetProperties().Length;
        //        //    for (int i = 1; i <= countProperties; i++)
        //        //    {
        //        //        if (i == countProperties)
        //        //        {
        //        //            AddImage(workSheet, recordIndex, i, m.QRCode);
        //        //        }
        //        //        else
        //        //            workSheet.Cells[recordIndex, i].Value = m.Content;
        //        //    }
        //        //    recordIndex++;
        //        //}

        //        //Add model titles to excel(Header)
        //        foreach (int i in modelTitles.Keys)
        //        {
        //            workSheet.Cells[1, i].Value = modelTitles[i];
        //        }

        //        //Format columns need to be Datetime
        //        if (colsDate != null)
        //        {
        //            foreach (int colnum in colsDate)
        //            {
        //                workSheet.Cells[2, colnum, model.Count + 1, colnum].Style.Numberformat.Format = "dd/MM/yyyy hh:mm:ss AM/PM";
        //            }
        //        }

        //        // By default, the column width is not  
        //        // set to auto fit for the content 
        //        // of the range, so we are using 
        //        // AutoFitColumns(minWidth, maxWidth) and Wraptext here.
        //        // Because we don't want column over width when render long text.
        //        workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns(15, 45);
        //        workSheet.Cells.Style.WrapText = true;
        //        excelPackage.Save();
        //        res = excelPackage.Stream;
        //    }
        //    return res;
        //}

        public Stream CreateQRExcel(Stream stream, List<DeviceViewModel.QRDevices> model, Dictionary<int, string> modelTitles, List<int> colsDate)
        {
            Stream res = null;
            // Creating an instance 
            // of ExcelPackage 
            using (var excel = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Setting the properties 
                // of the excel 
                excel.Workbook.Properties.Author = "Nguyen Minh Sang";
                excel.Workbook.Properties.Title = "IT Service Desk";
                excel.Workbook.Properties.Comments = "";

                // Name of the sheet 
                var workSheet = excel.Workbook.Worksheets.Add("First Sheet");

                // Setting the properties 
                // of the work sheet  
                workSheet.TabColor = Color.Black;
                workSheet.DefaultRowHeight = 12;

                // Setting the properties 
                // of the first row 
                workSheet.Row(1).Height = 20;
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Row(1).Style.Font.Bold = true;

                // Setting the properties 
                // of the data range
                // The content column
                workSheet.Column(1).Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                // of the QR column
                workSheet.Column(2).Width = 30;

                // Header of the Excel sheet 
                foreach (int i in modelTitles.Keys)
                {
                    workSheet.Cells[1, i].Value = modelTitles[i];
                }

                // Inserting the data into excel 
                // sheet by using the for each loop 
                // As we have values to the first row  
                // we will start with second row 
                int recordIndex = 2;

                foreach (var m in model)
                {
                    //workSheet.Row(recordIndex).Height = 140;
                    workSheet.Cells[recordIndex, 1].Value = m.Content;
                    AddImage(workSheet, recordIndex-1, 1, m.QRCode);
                    recordIndex++;
                }

                // By default, the column width is not  
                // set to auto fit for the content 
                // of the range, so we are using 
                // AutoFit() and Wraptext here.
                // Because we don't want column over width when render long text.
                workSheet.Column(1).AutoFit();
                workSheet.Cells.Style.WrapText = true;

                // Apply format table style for Data range
                // Create a range for the table
                ExcelRange range = workSheet.Cells[1, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                // Add a table to the range
                ExcelTable tab = workSheet.Tables.Add(range, "Table1");
                // Format the table
                tab.TableStyle = TableStyles.Medium2;

                excel.Save();
                res = excel.Stream;
            }
            return res;
        }

        private void AddImage(ExcelWorksheet oSheet, int rowIndex, int colIndex, string imagePath)
        {
            Bitmap image = new Bitmap(HostingEnvironment.MapPath(imagePath));
            ExcelPicture excelImage = null;
            if (image != null)
            {
                double rowHeight = PixelHeightToExcel(image.Height);

                oSheet.Row(rowIndex + 1).Height = PixelHeightToExcel(image.Height);
                string imageName = imagePath.Substring(imagePath.LastIndexOf("\\") + 2);
                excelImage = oSheet.Drawings.AddPicture(imageName, image);
                excelImage.From.Column = colIndex;
                excelImage.From.Row = rowIndex;
                // 2x2 px space for better alignment
                excelImage.From.ColumnOff = Pixel2MTU(5);
                excelImage.From.RowOff = Pixel2MTU(4);

            }
        }

        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }


        //public void CreateExcel()
        //{
        //    // Creating an instance 
        //    // of ExcelPackage 
        //    ExcelPackage excel = new ExcelPackage();

        //    // name of the sheet 
        //    var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

        //    // setting the properties 
        //    // of the work sheet  
        //    workSheet.TabColor = System.Drawing.Color.Black;
        //    workSheet.DefaultRowHeight = 12;

        //    // Setting the properties 
        //    // of the first row 
        //    workSheet.Row(1).Height = 20;
        //    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    workSheet.Row(1).Style.Font.Bold = true;

        //    // Header of the Excel sheet 
        //    workSheet.Cells[1, 1].Value = "S.No";
        //    workSheet.Cells[1, 2].Value = "Id";
        //    workSheet.Cells[1, 3].Value = "Name";

        //    // Inserting the article data into excel 
        //    // sheet by using the for each loop 
        //    // As we have values to the first row  
        //    // we will start with second row 
        //    int recordIndex = 2;

        //    foreach (var article in Articles)
        //    {
        //        workSheet.Cells[recordIndex, 1].Value = (recordIndex - 1).ToString();
        //        workSheet.Cells[recordIndex, 2].Value = article.Id;
        //        workSheet.Cells[recordIndex, 3].Value = article.Name;
        //        recordIndex++;
        //    }

        //    // By default, the column width is not  
        //    // set to auto fit for the content 
        //    // of the range, so we are using 
        //    // AutoFit() method here.  
        //    workSheet.Column(1).AutoFit();
        //    workSheet.Column(2).AutoFit();
        //    workSheet.Column(3).AutoFit();

        //    // file name with .xlsx extension  
        //    string p_strPath = "H:\\geeksforgeeks.xlsx";

        //    if (File.Exists(p_strPath))
        //        File.Delete(p_strPath);

        //    // Create excel file on physical disk  
        //    FileStream objFileStrm = File.Create(p_strPath);
        //    objFileStrm.Close();

        //    // Write content to excel file  
        //    File.WriteAllBytes(p_strPath, excel.GetAsByteArray());
        //    //Close Excel package 
        //    excel.Dispose();
        //    Console.ReadKey();
        //}

        /// <summary>
        /// Convert Image Width to Excel Width
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private double PixelWidthToExcel(int pixels)
        {
            var tempWidth = pixels * 0.14099;
            var correction = (tempWidth / 100) * -1.30;

            return tempWidth - correction;
        }

        /// <summary>
        /// Convert Image Height to Excel Height
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        private double PixelHeightToExcel(int pixels)
        {
            return pixels * 0.77;
        }

        private ExcelHelper() { }

        private static ExcelHelper _instance;

        public static ExcelHelper Instance { get { if (_instance == null) _instance = new ExcelHelper(); return _instance; } private set => _instance = value; }
    }
}