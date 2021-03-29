using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace govgameWebApp.Controllers
{
    public class GithubWebhookController : Controller
    {
        public IActionResult Index()
        {
            return Content("received github webhook successfully!");
        }
    }
}
