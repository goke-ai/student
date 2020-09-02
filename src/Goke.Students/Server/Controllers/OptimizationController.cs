using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Goke.Students.Shared;

namespace Goke.Students.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptimizationController : ControllerBase
    {

        private readonly ILogger<OptimizationController> logger;

        public OptimizationController(ILogger<OptimizationController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public int Get()
        {
            

            return 0;
        }

        [HttpGet("SimpleLp")]
        public OptimalParameters GetSimpleLp()
        {
            (double ObjValue, double X, double Y) = Goke.Optimization.Ortools.SimpleLpProgram();
            var optimalParameters = new OptimalParameters { ObjValue = ObjValue, X = X, Y = Y };

            return optimalParameters;
        }
    }
}
