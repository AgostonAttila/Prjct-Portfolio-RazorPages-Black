using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Helpers
{
    public interface IOfficeHelper
    {

        void SetCellValue(string text,int column,int row);

        void SetRangeBackGroundColor(System.Drawing.Color color, int startColumn, int startRow, int endColumn, int endRow);

        void SetRangeFontColor(System.Drawing.Color color, int startColumn, int startRow, int endColumn, int endRow);

        void SetRangeFontBold(bool isBold , int startColumn, int startRow, int endColumn, int endRow);

        void AutoFit(string startCell, string endCell);

        void CreateNewWorkBook(string name);

        void SaveWorkBook(string name,string path);

        void ReleaseObject(object obj);
       
    }
}
