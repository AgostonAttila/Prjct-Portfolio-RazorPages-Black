using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Helpers
{
    public class MsOfficeHelper : IOfficeHelper
    {
        public void CreateNewWorkBook(string name)
        {
            throw new NotImplementedException();
        }

        public void ReleaseObject(object obj)
        {
            throw new NotImplementedException();
        }

        public void SaveWorkBook(string name, string path)
        {
            throw new NotImplementedException();
        }

        public void SetRangeBackGroundColor(Color color, int startColumn, int startRow, int endColumn, int endRow)
        {
            throw new NotImplementedException();
        }

        public void SetRangeFontColor(Color color, int startColumn, int startRow, int endColumn, int endRow)
        {
            throw new NotImplementedException();
        }

        public void SetRangeFontBold(bool isBold, int startColumn, int startRow, int endColumn, int endRow)
        {
            throw new NotImplementedException();
        }

        public void  AutoFit(string startCell, string endCell)
        {
            throw new NotImplementedException();

            //Excel.Range range = xlWorkSheet.Range["A1", "E1"];
            //range.EntireColumn.AutoFit();
        }

        public void SetCellValue(string text, int column, int row)
        {
            throw new NotImplementedException();

            //Gray
            //xlWorkSheet.Cells[i, columnNumber] = sourceString;
            //if (i % 2 == 0)
            //{
            //    //formatRange = xlWorkSheet.get_Range(columnString + i, columnString + i);
            //    Excel.Range formatRange = xlWorkSheet.get_Range(xlWorkSheet.Cells[i, columnNumber], xlWorkSheet.Cells[i, columnNumber]);
            //    formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            //}

            //Black
            //xlWorkSheet.Cells[rowNumber, columnNumber] = sourceString;
            //Excel.Range formatRange = (Excel.Range)xlWorkSheet.Cells[rowNumber, columnNumber];
            //formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            //formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
        }

        
    }
}
