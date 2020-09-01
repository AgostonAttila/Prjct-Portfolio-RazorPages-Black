using PortfolioNetCore.Core;
using PortfolioNetCore.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.DAL
{
    public class UnitOfWork //: IUnitOfWork
    {
        /*
        private readonly PortfolioContext _context;

        public UnitOfWork(PortfolioContext context)
        {
            _context = context;
            Funds = new FundDBRepository(_context,null);
            Managements = new ManagementRepository(_context);
        }

        public IFundRepository Funds { get; private set; }

        public IManagementRepository Managements { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        */
    }
}
