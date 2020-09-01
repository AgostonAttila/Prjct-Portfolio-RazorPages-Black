using Microsoft.AspNetCore.Mvc;
using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioNetCore.Controllers
{
    public class ManagementController : Controller
    {
        //private readonly IUnitOfWork unitOfWork;
        //private readonly IMapper mapper;

        private readonly IManagementRepository repository;

        public ManagementController(IManagementRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("[action]")]
        public IEnumerable<Management> GetManagementList()
        {
            return repository.GetManagementList();
        }

        [Route("/api/[controller]/DeleteManagement/{name}")]
        [HttpPost("[action]")]
        public IEnumerable<Management> DeleteManagement(string name)
        {
            repository.DeleteManagement(name);
            return repository.GetManagementList();
        }


    }
}
