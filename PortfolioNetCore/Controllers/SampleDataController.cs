using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioNetCore.Core;
using PortfolioNetCore.Core.Model;

namespace PortfolioNetCore.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<Fund> GetFundList()
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);         

            //if (fundList == null)
            //    return NotFound();

            return null;

        }

    }
}
