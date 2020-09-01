using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;
using PortfolioNetCore.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PortfolioNetCore.Persistence
{

    public class FundTextRepository : IFundRepository
    {
        private IHostingEnvironment _hostingEnvironment;

    
        private string _contentDirectoryPath = "";
        private string _fundTargetFilePath = "";
        private string _fundSourceFilePath = "";

        public FundTextRepository(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
            _contentDirectoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
            _fundTargetFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "FundListResult.txt");
            _fundSourceFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "FundListSource.txt");
        }

        public IEnumerable<Fund> GetFundList()
        {
            List<Fund> fundList = new List<Fund>();
            if (Directory.Exists(_contentDirectoryPath) && File.Exists(_fundTargetFilePath))
            {
                var resultJson = File.ReadAllText(_fundTargetFilePath);
                fundList = JsonConvert.DeserializeObject<List<Fund>>(resultJson);
            }
            return fundList;
        }

        public IEnumerable<Fund> GetUpdatedFundList()
        {
            string[] fundArray = File.ReadAllLines(_fundSourceFilePath);
            SaveFundList(TeletraderHelper.GetDatasFromTeletrader(fundArray));
            return GetFundList();
        }

        public Fund SaveFund(Fund fund)
        {
            if (fund == null) return null;

            /* 
                        if (fund.MonthlyPerformanceList != null)
                            fund.MonthlyPerformanceAsString = JsonConvert.SerializeObject(fund.MonthlyPerformanceList);

                        fund.VolatilityArrayAsString = (fund.VolatilityArray != null) ? string.Join(";", fund.VolatilityArray) : "";
                        fund.SharpRateArrayAsString = (fund.SharpRateArray != null) ? string.Join(";", fund.SharpRateArray) : "";
                        fund.BestMonthArrayAsString = (fund.BestMonthArray != null) ? string.Join(";", fund.BestMonthArray) : "";
                        fund.WorstMonthArrayAsString = (fund.WorstMonthArray != null) ? string.Join(";", fund.WorstMonthArray) : "";
                        fund.MaxLossArrayAsString = (fund.MaxLossArray != null) ? string.Join(";", fund.MaxLossArray) : "";
                        fund.OverFulFilmentArrayAsString = (fund.OverFulFilmentArray != null) ? string.Join(";", fund.VolatilityArray) : "";
            */
            fund.FillStringProperties();
            
            IEnumerable<Fund> fundList = GetFundList();

            if (fundList != null)
                fundList.Append(fund);

            SaveFundList(fundList.ToList());

            return fund;
        }

        public IEnumerable<Fund> SaveFundList(List<Fund> fundList)
        {
            if (fundList == null) return null;

            List<Fund> dbFundList = GetFundList().ToList();

            List<string> isinNumberList = dbFundList.Select(b => b.ISINNumber).ToList();

            for (int i = 0; i < fundList.Count; i++)
            {
                Fund fund = fundList[i];

                if (fund == null) continue;
                /* 
                                if (fund.MonthlyPerformanceList != null)
                                    fund.MonthlyPerformanceAsString = JsonConvert.SerializeObject(fund.MonthlyPerformanceList);

                                fund.VolatilityArrayAsString = (fund.VolatilityArray != null) ? string.Join(";", fund.VolatilityArray) : "";
                                fund.SharpRateArrayAsString = (fund.SharpRateArray != null) ? string.Join(";", fund.SharpRateArray) : "";
                                fund.BestMonthArrayAsString = (fund.BestMonthArray != null) ? string.Join(";", fund.BestMonthArray) : "";
                                fund.WorstMonthArrayAsString = (fund.WorstMonthArray != null) ? string.Join(";", fund.WorstMonthArray) : "";
                                fund.MaxLossArrayAsString = (fund.MaxLossArray != null) ? string.Join(";", fund.MaxLossArray) : "";
                                fund.OverFulFilmentArrayAsString = (fund.OverFulFilmentArray != null) ? string.Join(";", fund.VolatilityArray) : "";
                                */

                fund.FillStringProperties();

                if (!isinNumberList.Contains(fund.ISINNumber))
                {
                    dbFundList.Add(fund);
                    isinNumberList.Add(fund.ISINNumber);
                }
                else
                {
                    Fund actFund = dbFundList.Where(b => b.ISINNumber == fund.ISINNumber).FirstOrDefault();
                    if (actFund != null) { fund = actFund; }

                }
            }

            if (!Directory.Exists(_contentDirectoryPath))
                Directory.CreateDirectory(_contentDirectoryPath);

            File.WriteAllText(_fundTargetFilePath, JsonConvert.SerializeObject(fundList));

            return GetFundList();

        }

        public bool DeleteFund(string ISIN)
        {
            List<Fund> dbFundList = GetFundList().ToList();
            Fund fund = dbFundList.Where(b => b.ISINNumber == ISIN).FirstOrDefault();
            if (fund != null) { dbFundList.Remove(fund); SaveFundList(dbFundList); return true; }

            return false;
        }

        public Fund GetFund(string isinNumber)
        {
            List<Fund> fundList = new List<Fund>();
            if (Directory.Exists(_contentDirectoryPath) && File.Exists(_fundTargetFilePath))
            {
                var resultJson = File.ReadAllText(_fundTargetFilePath);
                fundList = JsonConvert.DeserializeObject<List<Fund>>(resultJson);
            }
            return fundList.Where(p => p.ISINNumber == isinNumber).FirstOrDefault();
        }     
    }
}
