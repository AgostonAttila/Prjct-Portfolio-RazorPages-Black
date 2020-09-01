using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Core.Model
{
    public class Management
    {
        [Key]
        public string Name { get; set; }
        public string FundISINNumberString { get; set; }
        [NotMapped]
        public HashSet<string> FundISINNumberHashSet
        {
            get;// { return this.FundISINNumberString.Split(';').Distinct().ToHashSet(); }
            set;//{ }
        }
    }
}
