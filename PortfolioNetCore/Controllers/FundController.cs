using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;
using PortfolioNetCore.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortfolioNetCore.Controllers
{
    [Route("api/[controller]")]
    public class FundController : Controller
    {
      
        //private readonly IMapper mapper;
        private readonly IFundRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        private IHostingEnvironment _hostingEnvironment;

        private string _contentDirectoryPath = "";      

        public FundController(IFundRepository repository, IHostingEnvironment environment)
        {
            this._repository = repository;         
            _hostingEnvironment = environment;

            _contentDirectoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "App_Data");           
        }


        [HttpGet("[action]")]
        public IEnumerable<Fund> GetFundList()
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);         

            //if (fundList == null)
            //    return NotFound();

            //return _unitOfWork.Funds.GetAll();
         
            return _repository.GetFundList();
        }

        [Route("/api/[controller]/GetFund/{isinNumber}")]
        [HttpGet("[action]")]
        public Fund GetFund(string isinNumber)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);         

            //if (fundList == null)
            //    return NotFound();

            return _repository.GetFund(isinNumber);
        }

        [HttpGet("[action]")]
        public IEnumerable<Fund> GetUpdatedFundList()
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);         

            //if (fundList == null)
            //    return NotFound();

            return _repository.GetUpdatedFundList();
        }

        [HttpGet("[action]")]
        public Fund Test()
        {
            Fund result = new Fund();

            result.Name = _hostingEnvironment.ContentRootPath;
            result.Management = _hostingEnvironment.WebRootPath;

            try
            {

                WebClient client = new WebClient();

                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                Stream data = client.OpenRead("https://www.teletrader.com/tom-capital-growth-fund-f-chf/funds/details/FU_100047001");
                StreamReader reader = new StreamReader(data);
                result.Focus = reader.ReadToEnd();
                data.Close();
                reader.Close();


            }
            catch (Exception e) { result.Focus = e.InnerException.ToString(); }

            if (Directory.Exists(_hostingEnvironment.ContentRootPath))
            {
                string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "PFS_Result.txt");
                System.IO.File.WriteAllText(filePath, "ss");
                result.Currency = System.IO.File.ReadAllText(filePath);
            }

            result.ISINNumber = Directory.GetCurrentDirectory();


            return result;
        }




        [HttpGet("[action]")]
        public string GenerateExcelFundList()
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);         

            //if (fundList == null)
            //    return NotFound();
            string date = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day.ToString();
            string fileName = "Alapok_" + date + ".xlsx";             
         

            ExcelHelper.CreateNewWorkBook("Alapok_" + date);
            ExcelHelper.FillContent(GetFundList().ToList());
            ExcelHelper.SaveWorkBook(fileName, _contentDirectoryPath);

            return Path.Combine(_contentDirectoryPath, fileName); 
        }

        //[HttpGet("[action]")]
        //public IEnumerable<Fund> SavePortfolioFund(string filePath, List<Fund> fundList = null)
        //{
        //    fundList = Helper.GetTeletraderFundList(filePath, fundList);

        //    using (var db = new PortfolioContext())
        //    {
        //        for (int i = 0; i < fundList.Count; i++)
        //        {
        //            //fundList[i].VolatilityArrayString = String.Join(";", fundList[i].VolatilityArray);
        //            //fundList[i].SharpRateArrayString = String.Join(";", fundList[i].SharpRateArrayString);
        //            //fundList[i].BestMonthArrayString = String.Join(";", fundList[i].BestMonthArrayString);
        //            //fundList[i].WorstMonthArrayString = String.Join(";", fundList[i].WorstMonthArrayString);
        //            //fundList[i].MaxLossArrayString = String.Join(";", fundList[i].MaxLossArrayString);
        //            //fundList[i].OverFulFilmentArrayString = String.Join(";", fundList[i].OverFulFilmentArrayString);

        //            db.Funds.Add(fundList[i]);
        //        }
        //        var count = db.SaveChanges();
        //    }

        //    return fundList;
        //}

        //[HttpGet("[action]")]
        //public IEnumerable<Fund> ReFreshFundList()
        //{

        //    List<Fund> fundList = GetFundList().ToList();
        //    using (var db = new PortfolioContext())
        //    {

        //        List<FundDetail> dbFundDetailList = db.FundDetails.ToList();
        //        for (int i = 0; i < dbFundDetailList.Count; i++)
        //        {
        //            if (fundList.Where(p => p.Url == dbFundDetailList[i].Url).FirstOrDefault() == null)
        //                fundList.Add(new Fund { Url = dbFundDetailList[i].Url });
        //        }

        //        IEnumerable<Fund> fundList2 = SavePortfolioFund("", fundList);
        //        return fundList2;
        //    }
        //}
    }
}
