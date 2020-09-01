using PortfolioNetCore.Core.Model;
using PortfolioNetCore.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioNetCore.Core
{
    public interface IFundRepository // : IRepository<Fund>
    {
        Fund GetFund(string isinNumber);

        IEnumerable<Fund> GetFundList();

        IEnumerable<Fund> GetUpdatedFundList();

        IEnumerable<Fund> SaveFundList(List<Fund> fundList);

        Fund SaveFund(Fund fund);

        bool DeleteFund(string ISIN);
    }
}