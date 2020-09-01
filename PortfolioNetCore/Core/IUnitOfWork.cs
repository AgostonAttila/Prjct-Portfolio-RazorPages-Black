using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IFundRepository Funds { get; }
        IManagementRepository Managements { get; }     
        int Complete();

    }
}
