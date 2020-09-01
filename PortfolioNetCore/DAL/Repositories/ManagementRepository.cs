using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Persistence
{
    public class ManagementRepository : IManagementRepository
    {
        private readonly PortfolioContext context;

        public ManagementRepository(PortfolioContext context)
        {
            this.context = context;
        }

        public bool DeleteManagement(string name)
        {
            Management management = (Management)context.Managements.Where(p => p.Name == name).FirstOrDefault();
            if (management != null)
            {
                context.Managements.Remove(management);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IEnumerable<Management> GetManagementList()
        {
            return context.Managements.ToList();
        }
    }

}
