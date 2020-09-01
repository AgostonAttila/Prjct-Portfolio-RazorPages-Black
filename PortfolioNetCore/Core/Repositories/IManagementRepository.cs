using PortfolioNetCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Core
{
 
    public interface IManagementRepository
    {
        IEnumerable<Management> GetManagementList();

        bool DeleteManagement(string name);
    }
}
