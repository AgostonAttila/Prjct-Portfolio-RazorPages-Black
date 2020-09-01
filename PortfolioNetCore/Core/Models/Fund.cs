using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioNetCore.Core.Model
{
    public class Fund
    {
        public Fund()
        {
            if(this != null){

            this.MonthlyPerformanceList = (this.MonthlyPerformanceAsString != null) ? JsonConvert.DeserializeObject<List<MonthlyPerformance>>(this.MonthlyPerformanceAsString) : new List<MonthlyPerformance>();

            this.VolatilityArray = (!String.IsNullOrWhiteSpace(this.VolatilityArrayAsString)) ? this.VolatilityArrayAsString.Split(';').ToList() : new List<string>();
            this.SharpRateArray = (!String.IsNullOrWhiteSpace(this.SharpRateArrayAsString)) ? this.SharpRateArrayAsString.Split(';').ToList() : new List<string>();
            this.BestMonthArray = (!String.IsNullOrWhiteSpace(this.BestMonthArrayAsString)) ? this.BestMonthArrayAsString.Split(';').ToList() : new List<string>();
            this.WorstMonthArray = (!String.IsNullOrWhiteSpace(this.WorstMonthArrayAsString)) ? this.WorstMonthArrayAsString.Split(';').ToList() : new List<string>();
            this.MaxLossArray = (!String.IsNullOrWhiteSpace(this.MaxLossArrayAsString)) ? this.MaxLossArrayAsString.Split(';').ToList() : new List<string>();
            this.OverFulFilmentArray = (!String.IsNullOrWhiteSpace(this.OverFulFilmentArrayAsString)) ? this.OverFulFilmentArrayAsString.Split(';').ToList() : new List<string>();
            }
        }


        [Key]
        public string ISINNumber { get; set; } = "";
        public string Currency { get; set; } = "";
        public string Name { get; set; } = "";
        public string Management { get; set; } = "";
        public string Focus { get; set; } = "";
        public string Type { get; set; } = "";
        public string PerformanceYTD { get; set; } = "";
        public string Performance1Year { get; set; } = "";
        public string Performance3Year { get; set; } = "";
        public string Performance5Year { get; set; } = "";
        public string PerformanceFromBeggining { get; set; } = "";
        public string PerformanceActualMinus9 { get; set; } = "";
        public string PerformanceActualMinus8 { get; set; } = "";
        public string PerformanceActualMinus7 { get; set; } = "";
        public string PerformanceActualMinus6 { get; set; } = "";
        public string PerformanceActualMinus5 { get; set; } = "";
        public string PerformanceActualMinus4 { get; set; } = "";
        public string PerformanceActualMinus3 { get; set; } = "";
        public string PerformanceActualMinus2 { get; set; } = "";
        public string PerformanceActualMinus1 { get; set; } = "";
        public string PerformanceAverage { get; set; } = "";

        public string VolatilityArrayAsString { get; set; }
        public string SharpRateArrayAsString { get; set; }
        public string BestMonthArrayAsString { get; set; }
        public string WorstMonthArrayAsString { get; set; }
        public string MaxLossArrayAsString { get; set; }
        public string OverFulFilmentArrayAsString { get; set; }

        public string Url { get; set; }

        public string MonthlyPerformanceAsString { get; set; }


        [NotMapped]
        public List<MonthlyPerformance> MonthlyPerformanceList { get; set; }
        [NotMapped]
        public List<string> VolatilityArray { get; set; }
        [NotMapped]
        public List<string> SharpRateArray { get; set; }
        [NotMapped]
        public List<string> BestMonthArray { get; set; }
        [NotMapped]
        public List<string> WorstMonthArray { get; set; }
        [NotMapped]
        public List<string> MaxLossArray { get; set; }
        [NotMapped]
        public List<string> OverFulFilmentArray { get; set; }

        public void  FillStringProperties()
        {
            if (this.MonthlyPerformanceList != null)
                this.MonthlyPerformanceAsString = JsonConvert.SerializeObject(this.MonthlyPerformanceList);

            this.VolatilityArrayAsString = (this.VolatilityArray != null) ? string.Join(";", this.VolatilityArray) : "";
            this.SharpRateArrayAsString = (this.SharpRateArray != null) ? string.Join(";", this.SharpRateArray) : "";
            this.BestMonthArrayAsString = (this.BestMonthArray != null) ? string.Join(";", this.BestMonthArray) : "";
            this.WorstMonthArrayAsString = (this.WorstMonthArray != null) ? string.Join(";", this.WorstMonthArray) : "";
            this.MaxLossArrayAsString = (this.MaxLossArray != null) ? string.Join(";", this.MaxLossArray) : "";
            this.OverFulFilmentArrayAsString = (this.OverFulFilmentArray != null) ? string.Join(";", this.VolatilityArray) : "";
        }
    }



    public class MonthlyPerformance
    {
        public string Year { get; set; }
        public List<string> PerformanceListByMonth { get; set; }
        public string Performance { get; set; }
    }


}
