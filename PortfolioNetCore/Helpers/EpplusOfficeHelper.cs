using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Helpers
{
    //https://github.com/JanKallman/EPPlus/blob/master/SampleApp/Sample1.cs

    public class EpplusOfficeHelper : IOfficeHelper
    {

        private static ExcelPackage excelPackage = null;
        private static ExcelWorksheet worksheet = null;

        public  void CreateNewWorkBook(string sheetName)
        {
            excelPackage = new ExcelPackage();
           
                worksheet = excelPackage.Workbook.Worksheets.Add(sheetName);              
                //package.Workbook.Properties.Title = "Invertory";
                //package.Workbook.Properties.Author = "Jan Källman";
                //package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";
           
        }      

        public  void SaveWorkBook(string name, string path)
        {
            if (excelPackage != null)
                excelPackage.SaveAs(new FileInfo(path  + name));

            if (worksheet != null)
                ReleaseObject(worksheet);
            if (excelPackage != null)
                ReleaseObject(excelPackage);
        }

        public void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        public void AutoFit(string startCell, string endCell)
        {
            worksheet.Cells.AutoFitColumns(0);
        }
       

        public void SetCellValue(string text, int column, int row)
        {
            if (worksheet != null)
                worksheet.Cells[row, column].Value = text;
        }

        public void SetRangeBackGroundColor(Color color, int startColumn, int startRow, int endColumn, int endRow)
        {         
            if (worksheet != null)
                using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
                {
                
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(color);
                 
                }
        }

        public void SetRangeFontColor(Color color, int startColumn, int startRow, int endColumn, int endRow)
        {          
            if (worksheet != null)
                using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
                {                                 
                    range.Style.Font.Color.SetColor(color);
                }
        }

        public void SetRangeFontBold(bool isBold, int startColumn, int startRow, int endColumn, int endRow)
        {
            if (worksheet != null)
                using (var range = worksheet.Cells[startRow, startColumn, endRow, endColumn])
                {
                    range.Style.Font.Bold = isBold;                  
                }
        }
    }
}


