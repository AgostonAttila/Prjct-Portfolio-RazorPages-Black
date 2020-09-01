using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;
using PortfolioNetCore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Persistence
{

    public class FundDBRepository : Repository<Fund>, IFundRepository
    {

        private IHostingEnvironment _hostingEnvironment;
        private readonly PortfolioContext _context;

        private string _contentDirectoryPath = "";
        private string _fundTargetFilePath = "";
        private string _fundSourceFilePath = "";


        public FundDBRepository(PortfolioContext context, IHostingEnvironment environment) : base(context)
        {
            this._context = context;
            _hostingEnvironment = environment;
            _contentDirectoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");
            _fundTargetFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "FundListResult.txt");
            _fundSourceFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data", "FundListSource.txt");
        }

        public PortfolioContext PortfolioContext
        {
            get { return Context as PortfolioContext; }
        }

        public IEnumerable<Fund> GetFundList()
        {
            List<Fund> fundList = new List<Fund>();

            List<Fund> dbFundList = (_context.Funds != null) ? _context.Funds.ToList() : new List<Fund>();

            for (int i = 0; i < dbFundList.Count; i++)
            {
                fundList.Add(new Fund
                {
                    ISINNumber = dbFundList[i].ISINNumber,
                    Name = dbFundList[i].Name,
                    Currency = dbFundList[i].Currency,
                    Focus = dbFundList[i].Focus,
                    Management = dbFundList[i].Management,
                    Type = dbFundList[i].Type,
                    PerformanceYTD = dbFundList[i].PerformanceYTD,
                    Performance1Year = dbFundList[i].Performance1Year,
                    Performance3Year = dbFundList[i].Performance3Year,
                    Performance5Year = dbFundList[i].Performance5Year,
                    PerformanceFromBeggining = dbFundList[i].PerformanceFromBeggining,
                    PerformanceActualMinus1 = dbFundList[i].PerformanceActualMinus1,
                    PerformanceActualMinus2 = dbFundList[i].PerformanceActualMinus2,
                    PerformanceActualMinus3 = dbFundList[i].PerformanceActualMinus3,
                    PerformanceActualMinus4 = dbFundList[i].PerformanceActualMinus4,
                    PerformanceActualMinus5 = dbFundList[i].PerformanceActualMinus5,
                    PerformanceActualMinus6 = dbFundList[i].PerformanceActualMinus6,
                    PerformanceActualMinus7 = dbFundList[i].PerformanceActualMinus7,
                    PerformanceActualMinus8 = dbFundList[i].PerformanceActualMinus8,
                    PerformanceActualMinus9 = dbFundList[i].PerformanceActualMinus9,
                    PerformanceAverage = dbFundList[i].PerformanceAverage,

                    VolatilityArray = dbFundList[i].VolatilityArrayAsString != null ? dbFundList[i].VolatilityArrayAsString.Split(';').ToList() : new List<string>(),
                    SharpRateArray = dbFundList[i].SharpRateArray != null ? dbFundList[i].SharpRateArrayAsString.Split(';').ToList() : new List<string>(),
                    BestMonthArray = dbFundList[i].BestMonthArray != null ? dbFundList[i].BestMonthArrayAsString.Split(';').ToList() : new List<string>(),
                    WorstMonthArray = dbFundList[i].WorstMonthArray != null ? dbFundList[i].WorstMonthArrayAsString.Split(';').ToList() : new List<string>(),
                    MaxLossArray = dbFundList[i].MaxLossArray != null ? dbFundList[i].MaxLossArrayAsString.Split(';').ToList() : new List<string>(),
                    OverFulFilmentArray = dbFundList[i].OverFulFilmentArray != null ? dbFundList[i].OverFulFilmentArrayAsString.Split(';').ToList() : new List<string>(),

                    MonthlyPerformanceList = (!String.IsNullOrWhiteSpace(dbFundList[i].WorstMonthArrayAsString)) ? JsonConvert.DeserializeObject<List<MonthlyPerformance>>(dbFundList[i].MonthlyPerformanceAsString) : new List<MonthlyPerformance>()

                });
            }


            return fundList;
        }

        public IEnumerable<Fund> GetUpdatedFundList()
        {
            // string[] fundArray = new List<string> { "http://www.teletrader.com/hu/templeton-global-euro-fd-a-ydis/funds/details/FU_971655", "http://www.teletrader.com/hu/raiffeisen-osteuropa-aktien-r-t/funds/details/FU_80546", }.ToArray();

            string[] fundArray = File.ReadAllLines(_fundSourceFilePath);

            SaveFundList(TeletraderHelper.GetDatasFromTeletrader(fundArray));

            return GetFundList();
        }

        public Fund SaveFund(Fund fund)
        {
            if (fund == null) return null;

            if (fund.MonthlyPerformanceList != null)
                fund.MonthlyPerformanceAsString = JsonConvert.SerializeObject(fund.MonthlyPerformanceList);

            fund.VolatilityArrayAsString = (fund.VolatilityArray != null) ? string.Join(";", fund.VolatilityArray) : "";
            fund.SharpRateArrayAsString = (fund.SharpRateArray != null) ? string.Join(";", fund.SharpRateArray) : "";
            fund.BestMonthArrayAsString = (fund.BestMonthArray != null) ? string.Join(";", fund.BestMonthArray) : "";
            fund.WorstMonthArrayAsString = (fund.WorstMonthArray != null) ? string.Join(";", fund.WorstMonthArray) : "";
            fund.MaxLossArrayAsString = (fund.MaxLossArray != null) ? string.Join(";", fund.MaxLossArray) : "";
            fund.OverFulFilmentArrayAsString = (fund.OverFulFilmentArray != null) ? string.Join(";", fund.VolatilityArray) : "";

            _context.Funds.Add(fund);
            _context.SaveChanges();

            return fund;
        }

        public IEnumerable<Fund> SaveFundList(List<Fund> fundList)
        {
            if (fundList == null) return null;


            List<string> isinNumberList = _context.Funds.Select(b => b.ISINNumber).ToList();

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
                    _context.Funds.Add(fund);
                    isinNumberList.Add(fund.ISINNumber);
                }
                else
                {
                    Fund actFund = (Fund)_context.Funds.Where(b => b.ISINNumber == fund.ISINNumber).FirstOrDefault();
                    if (actFund != null) { fund = actFund; }

                }
            }
            _context.SaveChanges();

            return GetFundList();
        }

        public bool DeleteFund(string ISIN)
        {

            Fund fund = (Fund)_context.Funds.Where(b => b.ISINNumber == ISIN).FirstOrDefault();
            if (fund != null) { _context.Funds.Remove(fund); return true; }

            return false;
        }

        public Fund GetFund(string isinNumber)
        {

            Fund fund = _context.Funds?.Where(p => p.ISINNumber == isinNumber).FirstOrDefault();
            if (fund != null)
                fund.FillStringProperties();
            /*
                        if (fund != null)
                        {
                            fund.VolatilityArray = fund.VolatilityArrayAsString != null ? fund.VolatilityArrayAsString.Split(';').ToList() : new List<string>();
                            fund.SharpRateArray = fund.SharpRateArray != null ? fund.SharpRateArrayAsString.Split(';').ToList() : new List<string>();
                            fund.BestMonthArray = fund.BestMonthArray != null ? fund.BestMonthArrayAsString.Split(';').ToList() : new List<string>();
                            fund.WorstMonthArray = fund.WorstMonthArray != null ? fund.WorstMonthArrayAsString.Split(';').ToList() : new List<string>();
                            fund.MaxLossArray = fund.MaxLossArray != null ? fund.MaxLossArrayAsString.Split(';').ToList() : new List<string>();
                            fund.OverFulFilmentArray = fund.OverFulFilmentArray != null ? fund.OverFulFilmentArrayAsString.Split(';').ToList() : new List<string>();

                            fund.MonthlyPerformanceList = (!String.IsNullOrWhiteSpace(fund.WorstMonthArrayAsString)) ? JsonConvert.DeserializeObject<List<MonthlyPerformance>>(fund.MonthlyPerformanceAsString) : new List<MonthlyPerformance>();
                        }
          */
            return fund;
        }
    }
}
