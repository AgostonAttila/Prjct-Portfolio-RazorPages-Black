using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using PortfolioNetCore.Core.Model;
using HtmlAgilityPack;
using System.Net.Http;



namespace PortfolioNetCore
{
    class TeletraderHelper
    {
        static public int _actualYear = DateTime.Now.Year;

        //public static void GetExcelExportTeletrader(string excelFileName, string txtFile)
        //{

        //    if (!String.IsNullOrEmpty(excelFileName) && !String.IsNullOrEmpty(txtFile))
        //    {
        //        Excel.Application xlApplication = null;
        //        Excel.Workbook xlWorkBook = null;
        //        Excel.Worksheet xlWorkSheet = null;
        //        object misValue = System.Reflection.Missing.Value;

        //        try
        //        {
        //            xlApplication = new Excel.ApplicationClass();
        //            xlWorkBook = xlApplication.Workbooks.Add(misValue);
        //            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

        //            string[] fundArray = File.ReadAllLines(txtFile);
        //            if (fundArray != null)
        //            {
        //                progressBar.Maximum = fundArray.Count() * 2;
        //                progressBar.Step = 1;
        //                progressBar.Value = 0;
        //                progressBar.ForeColor = Color.SeaGreen;

        //                //Adatok leszedése    
        //                List<Fund> fundList = GetDatasFromTeletrader(progressBar, fundArray);

        //                //Task<List<Fund>> parallelTask = Task.Factory.StartNew(() => { List<Fund> fundList = GetDatasFromTeletrader(progressBar, fundArray); return fundList; });
        //                //parallelTask.Wait();

        //                //Excel feltöltése
        //                FillContent(xlWorkSheet, progressBar, fundList);
        //            }
        //            //Mentés                                       
        //            xlWorkBook.SaveAs(excelFileName, Excel.XlFileFormat.xlWorkbookDefault, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show("Error\n" + e.Message);
        //        }
        //        finally
        //        {
        //            progressBar.ForeColor = Color.Blue;
        //            if (xlWorkBook != null)
        //                xlWorkBook.Close(true, misValue, misValue);
        //            if (xlApplication != null)
        //                xlApplication.Quit();

        //            if (xlWorkSheet != null)
        //                releaseObject(xlWorkSheet);
        //            if (xlWorkBook != null)
        //                releaseObject(xlWorkBook);
        //            if (xlApplication != null)
        //                releaseObject(xlApplication);

        //        }
        //    }
        //    else
        //        MessageBox.Show("Input parameters arempty\n");
        //}

        public static List<Fund> GetTeletraderFundList(string filePath, List<Fund> fundList = null)
        {

            if (File.Exists(filePath))
            {
                string[] fundArray = (!String.IsNullOrWhiteSpace(filePath)) ? fundArray = File.ReadAllLines(filePath) : fundArray = fundList.Select(p => p.Url).ToArray();

                if (fundArray != null)
                {
                    //Adatok leszedése    
                    fundList = GetDatasFromTeletrader(fundArray);

                    //Task<List<Fund>> parallelTask = Task.Factory.StartNew(() => { List<Fund> fundList = GetDatasFromTeletrader(progressBar, fundArray); return fundList; });
                    //parallelTask.Wait();

                    fundList = fundList.Where(p => p != null).ToList();

                    return fundList;
                }
            }

            return null;
        }      


        public static List<Fund> GetDatasFromTeletrader(string[] fundArray)
        {

            List<Fund> fundList = new List<Fund>();
            //Parallel.For(0, fundArray.Count(), (k) =>
            for (int k = 0; k < fundArray.Count(); k++)
            {

                string sourceUrl = fundArray[k];
                string sourceString = "";

                if (!string.IsNullOrEmpty(sourceUrl) && sourceUrl.IndexOf("www.teletrader.com") != -1)
                {
                    //Weblap leszedése
                    sourceString = GetDataFromUrlWithoutDownload(sourceUrl.Trim());

                    string language = "";
                    List<string> sourcedStringList = new List<string>();
                    //if (!String.IsNullOrEmpty(GetPerformanceSumYearRegex(sourceString, "1 Év</th", 220)))
                    if (sourceString.IndexOf("1 Év</th") != -1)
                    {
                        language = "HU";
                        sourcedStringList = new List<string>() { "YTD</th", "1 Év</th", "3 Év</th", "5 Év</th", "Kezdettől</th", "Volatilitás", "Sharpe ráta", "Legjobb hónap", "Legrosszabb hónap", "Maximális veszteség", "Túlteljesítés" };
                    }
                    //else if (!String.IsNullOrEmpty(GetPerformanceSumYearRegex(sourceString, "1 Jahr</th", 220)))
                    else if (sourceString.IndexOf("1 Jahr</th") != -1)
                    {
                        language = "DE";
                        sourcedStringList = new List<string>() { "lfd. Jahr</th", "1 Jahr</th", "3 Jahre</th", "5 Jahre</th", "seit Beginn</th", "Volatilität", "Sharpe ratio", "Bester Monat", "Schlechtester Monat", "Maximaler Verlust", "Outperformance" };
                    }
                    //else if (!String.IsNullOrEmpty(GetPerformanceSumYearPointRegex(sourceString, "1 Year</th", 220)))
                    else if (sourceString.IndexOf("1 Year</th") != -1)
                    {
                        language = "EN";
                        sourcedStringList = new List<string>() { "YTD</th", "1 Year</th", "3 Years</th", "5 Years</th", "Since start</th", "Volatility", "Sharpe ratio", "Best month", "Worst month", "Maximum loss", "Outperformance" };
                    }

                    Fund actFund = new Fund();
                    if (language == "EN" || language == "HU" || language == "DE")
                    {
                        actFund.Currency = GetCurrencyRegex(sourceString, "cell-last fundCompany", 280);
                        actFund.Name = GetNameRegex(sourceString, "heading-detail", 150);
                        actFund.ISINNumber = GetISINRegex(sourceString, @"class=" + '"' + "isin", 100);
                        actFund.Management = GetManagementRegex(sourceString, "cell-last fundCompany", 100);
                        actFund.Focus = GetFocusRegex(sourceString, "investment-focus", 100);
                        actFund.Type = language == "EN" ? GetTypeRegexEnglish(sourceString, "yield", 50) : GetTypeRegex(sourceString, "yield", 50);

                        actFund.PerformanceYTD = language == "EN" ? GetPerformanceSumYearPointRegex(sourceString, sourcedStringList[0], 225) : GetPerformanceSumYearRegex(sourceString, sourcedStringList[0], 225);
                        actFund.Performance1Year = language == "EN" ? GetPerformanceSumYearPointRegex(sourceString, sourcedStringList[1], 220) : GetPerformanceSumYearRegex(sourceString, sourcedStringList[1], 220);
                        actFund.Performance3Year = language == "EN" ? GetPerformanceSumYearPointRegex(sourceString, sourcedStringList[2], 220) : GetPerformanceSumYearRegex(sourceString, sourcedStringList[2], 220);
                        actFund.Performance5Year = language == "EN" ? GetPerformanceSumYearPointRegex(sourceString, sourcedStringList[3], 220) : GetPerformanceSumYearRegex(sourceString, sourcedStringList[3], 220);
                        actFund.PerformanceFromBeggining = language == "EN" ? GetPerformanceFromBegginingPointRegex(sourceString, sourcedStringList[4], 225) : GetPerformanceFromBegginingRegex(sourceString, sourcedStringList[4], 225);

                        actFund.PerformanceActualMinus9 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 9).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 9).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus8 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 8).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 8).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus7 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 7).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 7).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus6 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 6).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 6).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus5 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 5).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 5).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus4 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 4).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 4).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus3 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 3).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 3).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus2 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 2).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 2).ToString() + "</th", 254);
                        actFund.PerformanceActualMinus1 = language == "EN" ? GetYearsPointRegex(sourceString, (_actualYear - 1).ToString() + "</th", 254) : GetYearsRegex(sourceString, (_actualYear - 1).ToString() + "</th", 254);
                        actFund.PerformanceAverage = String.Format("{0:0.##}", GetAverage(actFund.PerformanceActualMinus9, actFund.PerformanceActualMinus8, actFund.PerformanceActualMinus7, actFund.PerformanceActualMinus6, actFund.PerformanceActualMinus5, actFund.PerformanceActualMinus4, actFund.PerformanceActualMinus3, actFund.PerformanceActualMinus2, actFund.PerformanceActualMinus1));
                        actFund.VolatilityArray = GetVolatility(sourceString, sourcedStringList[5], 180);
                        actFund.SharpRateArray = GetVolatility(sourceString, sourcedStringList[6], 180);
                        actFund.BestMonthArray = GetOthers(sourceString, sourcedStringList[7], 300);
                        actFund.WorstMonthArray = GetOthers(sourceString, sourcedStringList[8], 300);
                        actFund.MaxLossArray = GetOthers(sourceString, sourcedStringList[9], 300);
                        actFund.OverFulFilmentArray = GetOthers(sourceString, sourcedStringList[10], 300);

                        actFund.Url = sourceUrl;


                        actFund.MonthlyPerformanceList = GetMonthlyPerformance(sourceString);


                        fundList.Add(actFund);
                    }
                    else
                        fundList.Add(null);
                }
                else
                    fundList.Add(null);

            }
            //});
            return fundList;
        }

        public static void GetDataFromUrlDownload(string urlAdress)
        {
            try
            {
                urlAdress = "http://www.teletrader.com/aegon-alfa-szarmaztatott-befektetesi-alap/funds/details/FU_5222";
                WebClient webClient = new WebClient();
                webClient.Encoding = System.Text.Encoding.UTF8;
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFile(urlAdress, @"D:\localfile.txt");
            }
            catch { }

        }

        static public string GetDataFromUrlWithoutDownload(string urlAdress)
        {
            try
            {

                WebClient client = new WebClient();

                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream data = client.OpenRead(urlAdress);
                StreamReader reader = new StreamReader(data);
                string resultString = reader.ReadToEnd();
                data.Close();
                reader.Close();

                return resultString;
            }
            catch (Exception e) { return null; }

        }

        public static bool IsInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #region Teletrader

        static public List<MonthlyPerformance> GetMonthlyPerformance(string html)
        {
            List<MonthlyPerformance> resultList = new List<MonthlyPerformance>();

            var htmlDoc = new HtmlDocument();
            string tableClassName = "component fund-monthly-performances-list";
            html = html.Substring(html.IndexOf(tableClassName));
            html = html.Replace(html.Substring(html.IndexOf("</table>")), "");
            htmlDoc.LoadHtml(html);

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//table//td");

            int counter = 0;
            if (htmlNodes != null)
            {
                MonthlyPerformance actMonthlyPerformance = null;
                for (int i = 0; i < htmlNodes.Count; i++)
                {
                    if (counter == 0)
                        actMonthlyPerformance = new MonthlyPerformance { Year = htmlNodes[i].InnerHtml, PerformanceListByMonth = new List<string>() };
                    else if (counter != 12)
                        actMonthlyPerformance.PerformanceListByMonth.Add(htmlNodes[i].InnerHtml);

                    counter++;
                    if (counter == 12)
                    {
                        if (htmlNodes.Count() < i + 1)
                            actMonthlyPerformance.Performance = htmlNodes[i + 1].InnerHtml;
                        resultList.Add(actMonthlyPerformance);
                        counter = 0;
                    }
                }

                return resultList;
            }
            return null;

        }


        static public string GetCurrencyRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;
                if (sourceText.IndexOf(searchedText) != -1)
                {
                    swapText = sourceText.Substring(sourceText.IndexOf(searchedText) + 25, cutLong);

                    Regex regData = new Regex(@"[A-Z]+");
                    Match matData = regData.Match(swapText);
                    string Data = matData.Value;
                    result = Data;
                    return result;
                }
            }
            catch
            {

            }
            return "";


        }

        static public string GetTypeRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                if (sourceText.LastIndexOf(searchedText) != -1)
                {
                    string swapText, result;

                    swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText) + 7, cutLong);
                    swapText = swapText.Replace("&#218;", "U"); //Ú
                    swapText = swapText.Replace("&#233;", "e"); //é

                    Regex regData = new Regex(@"[A-Z|a-z]*");
                    Match matData = regData.Match(swapText);
                    string Data = matData.Value;
                    result = Data;
                    return result;
                }
            }
            catch
            {

            }
            return "";
        }

        static public string GetTypeRegexEnglish(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText) + 21, cutLong);
                swapText = swapText.Replace("<th>", "");
                swapText = swapText.ToLower();

                Regex regData = new Regex(@"[A-Z|a-z]*");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;
                return result;
            }
            catch
            {
                return "";

            }
        }

        static public string GetManagementRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                if (sourceText.LastIndexOf(searchedText) != -1)
                {
                    string swapText, result;

                    swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText) + 21, cutLong);
                    swapText = WebUtility.HtmlDecode(swapText);

                    Regex regData = new Regex(@"[A-Z]+[a-z|á|é|ö\s\w]+[A-Z]+[a-z]*");
                    Match matData = regData.Match(swapText);
                    string Data = matData.Value;
                    result = Data;

                    return result;
                }
            }
            catch
            {
            }
            return "";


        }

        static public string GetFocusRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                if (sourceText.LastIndexOf(searchedText) != -1)
                {
                    string swapText, result;

                    swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText), cutLong);

                    Regex regData = new Regex(@"[A-Z]+[a-z|�|é|ö|ú|á]*");
                    Match matData = regData.Match(swapText);
                    string Data = matData.Value;
                    result = Data;

                    return result;
                }
            }
            catch
            {

            }
            return "";
        }

        static public string GetISINRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;
                if (sourceText.IndexOf(searchedText) != -1)
                {
                    swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                    swapText = swapText.Substring(swapText.IndexOf(">/") + 2, 30);

                    Regex regData = new Regex(@"[A-Z]+[0-9]*");
                    Match matData = regData.Match(swapText);
                    string Data = matData.Value;
                    result = Data;

                    return result;
                }
            }
            catch
            {

            }
            return "";
        }

        static public string GetNameRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;
                if (sourceText.IndexOf(searchedText) != -1)
                {
                    swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                    int index = swapText.IndexOf("<span class");

                    if (index != -1)
                    {
                        int firstIndex = swapText.IndexOf(">") + 1;
                        result = swapText.Substring(firstIndex, (index != -1) ? index - firstIndex : 66);
                        //result = swapText.Replace(swapText.Substring(index, swapText.Length - index), "");
                    }
                    else
                    {
                        Regex regData = new Regex(@"[a-z|ö|é|ó|-|A-Z|\s|\w.]*");
                        Match matData = regData.Match(swapText);
                        string Data = matData.Value;
                        result = Data;
                    }
                    return result;
                }
            }
            catch
            {

            }
            return "";
        }

        static public string GetPerformanceSumYearRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 30);
                //swapText = swapText.Substring(swapText.IndexOf(">") + 1, 11);

                Regex regData = new Regex(@"[+|-]+\d+,+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;

                return result;
            }
            catch
            {
                return "";

            }

        }
        //uaz mint az előző
        static public string GetPerformanceFromBegginingRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 15);
                //swapText = swapText.Substring(swapText.IndexOf(">") + 1, 11);

                Regex regData = new Regex(@"[+|-]+\d+,+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;

                return result;
            }
            catch
            {
                return "";

            }
        }

        static public string GetYearsRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 15);
                swapText = swapText.Substring(swapText.IndexOf(">") + 1, 8);

                Regex regData = new Regex(@"[+|-]+\d+,+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;

                return result;
            }
            catch
            {
                return "";

            }
        }

        static public List<string> GetVolatility(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText = "";
                List<string> result = new List<string>();

                if (sourceText.IndexOf(searchedText) != -1)
                {
                    swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText), cutLong);
                    if (swapText.IndexOf("</tr>") != -1)
                        swapText = swapText.Substring(0, swapText.IndexOf("</tr>"));

                    swapText = swapText.Replace(searchedText + "</td>", "");
                    swapText = swapText.Replace("<td>", "");
                    swapText = swapText.Replace("</td>", "");
                    swapText = swapText.Replace("<td class=" + '"' + "cell-last" + '"' + ">", "");
                    swapText = Regex.Replace(swapText, @"\t|\r", "");
                    swapText = Regex.Replace(swapText, @"\n", " ");
                    swapText = swapText.Trim();
                    string[] digits = Regex.Split(swapText, @"\s+");

                    return digits.ToList();
                }
                else
                    return null;
            }
            catch
            {
                return null;

            }
        }

        static public List<string> GetOthers(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText = "";
                List<string> result = new List<string>();

                if (sourceText.IndexOf(searchedText) != -1)
                {
                    swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                    if (swapText.IndexOf("</tr>") != -1)
                        swapText = swapText.Substring(0, swapText.IndexOf("</tr>"));

                    swapText = swapText.Replace(searchedText + "</td>", "");
                    swapText = swapText.Replace("<td>", "");
                    swapText = swapText.Replace("</td>", "");
                    swapText = swapText.Replace("<span class=" + '"' + "up" + '"' + ">", "");
                    swapText = swapText.Replace("<span class=" + '"' + "down" + '"' + ">", "");
                    swapText = swapText.Replace("<span class=" + '"' + "cell-last" + '"' + ">", "");
                    swapText = swapText.Replace("<td class=" + '"' + "cell-last" + '"' + ">", "");
                    swapText = swapText.Replace("<span class=" + '"' + "nochange" + '"' + ">", "");
                    swapText = swapText.Replace("</span>", "");
                    swapText = Regex.Replace(swapText, @"\t|\r", "");
                    swapText = Regex.Replace(swapText, @"\n", " ");
                    swapText = swapText.Trim();
                    string[] digits = Regex.Split(swapText, @"\s+");

                    return digits.ToList();
                }
                else
                    return null;
            }
            catch
            {
                return null;

            }
        }

        //Angol
        static public string GetPerformanceSumYearPointRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.LastIndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 30);
                //swapText = swapText.Substring(swapText.IndexOf(">") + 1, 11);

                Regex regData = new Regex(@"[+|-]+\d+.+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;
                result = result.Replace(".", ",");

                return result;
            }
            catch
            {
                return "";

            }

        }

        static public string GetPerformanceFromBegginingPointRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 15);
                //  swapText = swapText.Substring(swapText.IndexOf(">") + 1, 11);

                Regex regData = new Regex(@"[+|-]+\d+.+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;
                result = result.Replace(".", ",");

                return result;
            }
            catch
            {
                return "";

            }
        }

        static public string GetYearsPointRegex(string sourceText, string searchedText, int cutLong)
        {
            try
            {
                string swapText, result;

                swapText = sourceText.Substring(sourceText.IndexOf(searchedText), cutLong);
                swapText = swapText.Substring(swapText.Length - 15);
                swapText = swapText.Substring(swapText.IndexOf(">") + 1, 8);

                Regex regData = new Regex(@"[+|-]+\d+.+\d+");
                Match matData = regData.Match(swapText);
                string Data = matData.Value;
                result = Data;
                result = result.Replace(".", ",");

                return result;
            }
            catch
            {
                return "";

            }
        }

        //egyeb
        static public bool GetLinkRegex(string sourceText)
        {
            try
            {

                Regex regData = new Regex("www.teletrader.com/+[a-z|-]*");
                Match matData = regData.Match(sourceText);
                string Data = matData.Value;
                if (!String.IsNullOrEmpty(Data))
                {
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;

            }
        }
        #endregion  

        #region Atlag      
        static public double GetAverage(string Performance2006, string Performance2007, string Performance2008, string Performance2009, string Performance2010, string Performance2011, string Performance2012, string Performance2013, string Performance2014)
        {
            try
            {
                double result = 0;

                double swapPerformance2005;
                double swapPerformance2006;
                double swapPerformance2007;
                double swapPerformance2008;
                double swapPerformance2009;
                double swapPerformance2010;
                double swapPerformance2011;
                double swapPerformance2012;
                double swapPerformance2013;

                //2014
                try
                {
                    if (Performance2014[0] == '-')
                    {
                        Performance2014 = RegexGetAverage(Performance2014);
                        Performance2014 = Performance2014.Replace(',', '.');
                        swapPerformance2005 = -1 * double.Parse(Performance2014, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2014 = RegexGetAverage(Performance2014);
                        Performance2014 = Performance2014.Replace(",", ".");
                        swapPerformance2005 = double.Parse(Performance2014, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2005 = 0;

                }
                //2006
                try
                {
                    if (Performance2006[0] == '-')
                    {
                        Performance2006 = RegexGetAverage(Performance2006);
                        Performance2006 = Performance2006.Replace(',', '.');
                        swapPerformance2006 = -1 * double.Parse(Performance2006, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2006 = RegexGetAverage(Performance2006);
                        Performance2006 = Performance2006.Replace(",", ".");
                        swapPerformance2006 = double.Parse(Performance2006, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2006 = 0;

                }
                //2007
                try
                {
                    if (Performance2007[0] == '-')
                    {
                        Performance2007 = RegexGetAverage(Performance2007);
                        Performance2007 = Performance2007.Replace(',', '.');
                        swapPerformance2007 = -1 * double.Parse(Performance2007, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2007 = RegexGetAverage(Performance2007);
                        Performance2007 = Performance2007.Replace(",", ".");
                        swapPerformance2007 = double.Parse(Performance2007, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2007 = 0;

                }
                //2008
                try
                {
                    if (Performance2008[0] == '-')
                    {
                        Performance2008 = RegexGetAverage(Performance2008);
                        Performance2008 = Performance2008.Replace(',', '.');
                        swapPerformance2008 = -1 * double.Parse(Performance2008, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2008 = RegexGetAverage(Performance2008);
                        Performance2008 = Performance2008.Replace(",", ".");
                        swapPerformance2008 = double.Parse(Performance2008, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2008 = 0;

                }
                //2009
                try
                {
                    if (Performance2009[0] == '-')
                    {
                        Performance2009 = RegexGetAverage(Performance2009);
                        Performance2009 = Performance2009.Replace(',', '.');
                        swapPerformance2009 = -1 * double.Parse(Performance2009, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2009 = RegexGetAverage(Performance2009);
                        Performance2009 = Performance2009.Replace(",", ".");
                        swapPerformance2009 = double.Parse(Performance2009, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2009 = 0;

                }
                //2010
                try
                {
                    if (Performance2010[0] == '-')
                    {
                        Performance2010 = RegexGetAverage(Performance2010);
                        Performance2010 = Performance2010.Replace(',', '.');
                        swapPerformance2010 = -1 * double.Parse(Performance2010, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2010 = RegexGetAverage(Performance2010);
                        Performance2010 = Performance2010.Replace(",", ".");
                        swapPerformance2010 = double.Parse(Performance2010, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2010 = 0;

                }
                //2011
                try
                {
                    if (Performance2011[0] == '-')
                    {
                        Performance2011 = RegexGetAverage(Performance2011);
                        Performance2011 = Performance2011.Replace(',', '.');
                        swapPerformance2011 = -1 * double.Parse(Performance2011, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2011 = RegexGetAverage(Performance2011);
                        Performance2011 = Performance2011.Replace(",", ".");
                        swapPerformance2011 = double.Parse(Performance2011, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2011 = 0;

                }
                //2012
                try
                {
                    if (Performance2012[0] == '-')
                    {
                        Performance2012 = RegexGetAverage(Performance2012);
                        Performance2012 = Performance2012.Replace(',', '.');
                        swapPerformance2012 = -1 * double.Parse(Performance2012, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2012 = RegexGetAverage(Performance2012);
                        Performance2012 = Performance2012.Replace(",", ".");
                        swapPerformance2012 = double.Parse(Performance2012, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2012 = 0;

                }
                //2013
                try
                {
                    if (Performance2013[0] == '-')
                    {
                        Performance2013 = RegexGetAverage(Performance2013);
                        Performance2013 = Performance2013.Replace(',', '.');
                        swapPerformance2013 = -1 * double.Parse(Performance2013, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2013 = RegexGetAverage(Performance2013);
                        Performance2013 = Performance2013.Replace(",", ".");
                        swapPerformance2013 = double.Parse(Performance2013, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2013 = 0;

                }
                //Átlag kiszámítása

                int piece = 0;

                if (swapPerformance2005 != 0)
                {
                    result = result + swapPerformance2005;
                    piece += 1;
                }
                if (swapPerformance2006 != 0)
                {
                    result = result + swapPerformance2006;
                    piece += 1;
                }
                if (swapPerformance2007 != 0)
                {
                    result = result + swapPerformance2007;
                    piece += 1;
                }
                if (swapPerformance2008 != 0)
                {
                    result = result + swapPerformance2008;
                    piece += 1;
                }
                if (swapPerformance2009 != 0)
                {
                    result = result + swapPerformance2009;
                    piece += 1;
                }
                if (swapPerformance2010 != 0)
                {
                    result = result + swapPerformance2010;
                    piece += 1;
                }
                if (swapPerformance2011 != 0)
                {
                    result = result + swapPerformance2011;
                    piece += 1;
                }
                if (swapPerformance2012 != 0)
                {
                    result = result + swapPerformance2012;
                    piece += 1;
                }
                if (swapPerformance2013 != 0)
                {
                    result = result + swapPerformance2013;
                    piece += 1;
                }

                if (piece != 0)
                {
                    result = result / piece;
                    string swapResult = String.Format("{0:0.##}", result);
                    swapResult = swapResult.Replace(',', '.');
                    result = double.Parse(swapResult, System.Globalization.CultureInfo.InvariantCulture);
                }
                else result = 0;

                return result;
            }
            catch
            {

                return 0;
            }

        }

        static public double GetAverageDot(string Performance2006, string Performance2007, string Performance2008, string Performance2009, string Performance2010, string Performance2011, string Performance2012, string Performance2013, string Performance2014)
        {
            try
            {
                double result = 0;

                double swapPerformance2006;
                double swapPerformance2007;
                double swapPerformance2008;
                double swapPerformance2009;
                double swapPerformance2010;
                double swapPerformance2011;
                double swapPerformance2012;
                double swapPerformance2013;
                double swapPerformance2014;

                //20114
                try
                {
                    if (Performance2014[0] == '-')
                    {
                        Performance2014 = RegexGetAverage(Performance2014);
                        swapPerformance2014 = -1 * double.Parse(Performance2014, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2014 = RegexGetAverage(Performance2014);
                        swapPerformance2014 = double.Parse(Performance2014, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2014 = 0;

                }
                //2006
                try
                {
                    if (Performance2006[0] == '-')
                    {
                        Performance2006 = RegexGetAverage(Performance2006);
                        swapPerformance2006 = -1 * double.Parse(Performance2006, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2006 = RegexGetAverage(Performance2006);
                        swapPerformance2006 = double.Parse(Performance2006, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2006 = 0;

                }
                //2007
                try
                {
                    if (Performance2007[0] == '-')
                    {
                        Performance2007 = RegexGetAverage(Performance2007);
                        swapPerformance2007 = -1 * double.Parse(Performance2007, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2007 = RegexGetAverage(Performance2007);
                        swapPerformance2007 = double.Parse(Performance2007, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2007 = 0;

                }
                //2008
                try
                {
                    if (Performance2008[0] == '-')
                    {
                        Performance2008 = RegexGetAverage(Performance2008);
                        swapPerformance2008 = -1 * double.Parse(Performance2008, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2008 = RegexGetAverage(Performance2008);
                        swapPerformance2008 = double.Parse(Performance2008, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2008 = 0;

                }
                //2009
                try
                {
                    if (Performance2009[0] == '-')
                    {
                        Performance2009 = RegexGetAverage(Performance2009);
                        swapPerformance2009 = -1 * double.Parse(Performance2009, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2009 = RegexGetAverage(Performance2009);
                        swapPerformance2009 = double.Parse(Performance2009, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2009 = 0;

                }
                //2010
                try
                {
                    if (Performance2010[0] == '-')
                    {
                        Performance2010 = RegexGetAverage(Performance2010);
                        swapPerformance2010 = -1 * double.Parse(Performance2010, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2010 = RegexGetAverage(Performance2010); ;
                        swapPerformance2010 = double.Parse(Performance2010, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2010 = 0;

                }
                //2011
                try
                {
                    if (Performance2011[0] == '-')
                    {
                        Performance2011 = RegexGetAverage(Performance2011);
                        swapPerformance2011 = -1 * double.Parse(Performance2011, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2011 = RegexGetAverage(Performance2011);
                        swapPerformance2011 = double.Parse(Performance2011, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2011 = 0;

                }
                //2012
                try
                {
                    if (Performance2012[0] == '-')
                    {
                        Performance2012 = RegexGetAverage(Performance2012);
                        swapPerformance2012 = -1 * double.Parse(Performance2012, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2012 = RegexGetAverage(Performance2012);
                        swapPerformance2012 = double.Parse(Performance2012, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2012 = 0;

                }
                //2013
                try
                {
                    if (Performance2013[0] == '-')
                    {
                        Performance2013 = RegexGetAverage(Performance2013);
                        swapPerformance2013 = -1 * double.Parse(Performance2013, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Performance2013 = RegexGetAverage(Performance2013);
                        swapPerformance2013 = double.Parse(Performance2013, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                catch
                {
                    swapPerformance2013 = 0;

                }
                //Átlag kiszámítása

                int piece = 0;

                if (swapPerformance2014 != 0)
                {
                    result = result + swapPerformance2014;
                    piece += 1;
                }
                if (swapPerformance2006 != 0)
                {
                    result = result + swapPerformance2006;
                    piece += 1;
                }
                if (swapPerformance2007 != 0)
                {
                    result = result + swapPerformance2007;
                    piece += 1;
                }
                if (swapPerformance2008 != 0)
                {
                    result = result + swapPerformance2008;
                    piece += 1;
                }
                if (swapPerformance2009 != 0)
                {
                    result = result + swapPerformance2009;
                    piece += 1;
                }
                if (swapPerformance2010 != 0)
                {
                    result = result + swapPerformance2010;
                    piece += 1;
                }
                if (swapPerformance2011 != 0)
                {
                    result = result + swapPerformance2011;
                    piece += 1;
                }
                if (swapPerformance2012 != 0)
                {
                    result = result + swapPerformance2012;
                    piece += 1;
                }
                if (swapPerformance2013 != 0)
                {
                    result = result + swapPerformance2013;
                    piece += 1;
                }

                if (piece != 0)
                {
                    result = result / piece;
                    string swapResult = String.Format("{0:0.##}", result);
                    swapResult = swapResult.Replace(',', '.');
                    result = double.Parse(swapResult, System.Globalization.CultureInfo.InvariantCulture);
                }
                else result = 0;

                return result;
            }
            catch
            {

                return 0;
            }

        }

        static public string RegexGetAverage(string sourceString)
        {
            try
            {
                Regex regData = new Regex("");
                if (sourceString.Contains(','))
                {
                    regData = new Regex(@"\d+,+\d+");
                }
                else regData = new Regex(@"\d+.+\d+");

                Match matData = regData.Match(sourceString);
                string Data = matData.Value;
                return Data;
            }
            catch { return null; }

        }
        #endregion

    }
}
