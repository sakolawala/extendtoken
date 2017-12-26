using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace icy2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("<h2>Identity Server is up and running...</h2>", "text/html");
        }
    }
}