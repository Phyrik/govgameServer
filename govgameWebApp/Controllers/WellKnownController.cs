using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;

namespace govgameWebApp.Controllers
{
    public class WellKnownController : Controller
    {
        private readonly IWebHostEnvironment env;

        public WellKnownController(IWebHostEnvironment hostingEnvironment)
        {
            this.env = hostingEnvironment;
        }

        public IActionResult Index(string path)
        {
            string textToReturn = System.IO.File.ReadAllText(System.IO.Path.Combine(env.WebRootPath, ".well-known", path));
            return Content(textToReturn, "text/plain");
        }
    }
}
