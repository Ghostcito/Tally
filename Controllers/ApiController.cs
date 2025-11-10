using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : ControllerBase
    {
        
    }
}