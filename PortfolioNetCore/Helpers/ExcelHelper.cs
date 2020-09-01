using PortfolioNetCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Helpers
{
    public class ExcelHelper
    {
        private static readonly IOfficeHelper officeHelper = new EpplusOfficeHelper();
        readonly static int _actualYear = DateTime.Now.Year;

        private static readonly System.Drawing.Color positiveValueColor = System.Drawing.Color.LightGreen;
        private static readonly System.Drawing.Color negativeValueColor = System.Drawing.Color.IndianRed;
        private static readonly System.Drawing.Color noneValueColor = System.Drawing.Color.DimGray;
        private static readonly System.Drawing.Color blackColor = System.Drawing.Color.Black;
        private static readonly System.Drawing.Color fontColor = System.Drawing.Color.White;
        private static readonly System.Drawing.Color headerColor = System.Drawing.Color.DimGray;

        public static void CreateNewWorkBook(string sheetName)
        {
            officeHelper.CreateNewWorkBook(sheetName);
        }

        public static void SaveWorkBook(string fileName, string path)
        {
            officeHelper.SaveWorkBook(fileName, path);
        }


        public static void FillContent(List<Fund> fundList)
        {

           



            #region Header
            List<string> headerTitleList = new List<string> { "Alap neve", "ISIN", "Valuta", "Menedzsment", "Fókusz", "1 éves", "3 éves", "5 éves", "Kezdettől" ,"",
                (_actualYear - 9).ToString(),(_actualYear - 8).ToString(),(_actualYear - 7).ToString(),(_actualYear - 6).ToString(),(_actualYear - 5).ToString(),(_actualYear - 4).ToString(),(_actualYear - 3).ToString(),(_actualYear - 2).ToString(),(_actualYear - 1).ToString(),
                "YTD",
                "Volatilitás YTD","Volatilitás 6 hónap","Volatilitás 1 év","Volatilitás 3 év","Volatilitás 5 év",
                "Sharpe ráta  YTD","Sharpe ráta 6 hónap","Sharpe ráta 1 év","Sharpe ráta 3 év","Sharpe ráta 5 év",
                "Legjobb hónap YTD",           "Legjobb hónap 6 hónap",           "Legjobb hónap 1 év",            "Legjobb hónap 3 év",           "Legjobb hónap 5 év",
                "Legrosszabb hónap YTD",             "Legrosszabb hónap 6 hónap",            "Legrosszabb hónap 1 év",           "Legrosszabb hónap 3 év",            "Legrosszabb hónap 5 év",
                "Maximális veszteség YTD",           "Maximális veszteség 6 hónap",           "Maximális veszteség 1 év",            "Maximális veszteség 3 év",           "Maximális veszteség 5 év",
                "Túlteljesítés YTD",           "Túlteljesítés 6 hónap",            "Túlteljesítés 1 év",           "Túlteljesítés 3 év",           "Túlteljesítés 5 év"                };

            officeHelper.SetRangeBackGroundColor(headerColor, 1, 1, 50, 1);
            officeHelper.SetRangeFontColor(fontColor, 1, 1, 50, 1);
            officeHelper.SetRangeFontBold(true, 1, 1, 50, 1);

            for (int j = 0; j < headerTitleList.Count; j++)
                officeHelper.SetCellValue(headerTitleList[j], j + 1, 1);

            #endregion

            int i = 0;
            for (int j = 0; j < fundList.Count; j++)
            {
                i = j + 2;

                try
                {

                    if (fundList[j] != null)
                    {
                        List<string> textList = new List<string> { fundList[j].Name, fundList[j].ISINNumber, fundList[j].Currency, fundList[j].Management, fundList[j].Focus, fundList[j].Performance1Year, fundList[j].Performance3Year, fundList[j].Performance5Year, fundList[j].PerformanceFromBeggining, " ", fundList[j].PerformanceActualMinus9, fundList[j].PerformanceActualMinus8, fundList[j].PerformanceActualMinus7, fundList[j].PerformanceActualMinus6, fundList[j].PerformanceActualMinus5, fundList[j].PerformanceActualMinus4, fundList[j].PerformanceActualMinus3, fundList[j].PerformanceActualMinus2, fundList[j].PerformanceActualMinus1, fundList[j].PerformanceYTD };

                        for (int k = 0; k < textList.Count; k++)
                        {
                            int column = k + 1;
                            officeHelper.SetCellValue(textList[k], column, i);


                            if (column > 5) // column < 51)
                            {
                                System.Drawing.Color color = noneValueColor;
                                if (!String.IsNullOrWhiteSpace(textList[k]))
                                    color = textList[k].Contains("-") ? negativeValueColor : positiveValueColor;

                                officeHelper.SetRangeBackGroundColor(color, column, i, column, i);
                            }

                        }

                        FilllListTypeCells(fundList[j].VolatilityArray, i, j, 21, true);
                        FilllListTypeCells(fundList[j].SharpRateArray, i, j, 26, true);
                        FilllListTypeCells(fundList[j].BestMonthArray, i, j, 31);
                        FilllListTypeCells(fundList[j].WorstMonthArray, i, j, 36);
                        FilllListTypeCells(fundList[j].MaxLossArray, i, j, 41);
                        FilllListTypeCells(fundList[j].OverFulFilmentArray, i, j, 46);

                    }
                    else
                        officeHelper.SetRangeBackGroundColor(blackColor, 1, i, 50, i);
                }
                catch (Exception)
                {
                }
            }

            officeHelper.SetRangeBackGroundColor(headerColor, 1, 2, 1, fundList.Count+1);
            officeHelper.SetRangeFontColor(fontColor, 1, 2, 1, fundList.Count+1);
            officeHelper.SetRangeFontBold(true, 1, 2, 1, fundList.Count+1);

            officeHelper.AutoFit("A1", "E1");
        }

        private static void FilllListTypeCells(List<string> stringList, int i, int j, int plusNumber, bool different = false)
        {

            if (stringList != null)
            {
                for (int k = 0; k < 5; k++)
                {
                    string text = stringList[k].ToString();
                    if (!different && stringList[k].ToString().IndexOf("%") == -1) text = "";
                    {
                        officeHelper.SetCellValue(text, k + plusNumber, i);

                        System.Drawing.Color color = noneValueColor;
                        if (!String.IsNullOrWhiteSpace(text))
                            color = text.Contains("-") ? negativeValueColor : positiveValueColor;

                        officeHelper.SetRangeBackGroundColor(color, k + plusNumber, i, k + plusNumber, i);
                    }
                }
            }
            else
            {
                for (int k = 0; k < 5; k++)
                {
                    officeHelper.SetCellValue(" ", k + plusNumber, i);
                    officeHelper.SetRangeBackGroundColor(noneValueColor, k + plusNumber, i, k + plusNumber, i);
                }
            }

        }

    }
}
