using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;

namespace govgameWebApp.Controllers
{
    public class AutoDeployerController : Controller
    {
        [HttpPost]
        public IActionResult Deploy()
        {
            JObject requestBodyJObject = JObject.Parse(new StreamReader(Request.Body).ReadToEndAsync().Result);
            if (requestBodyJObject["ref"].ToString().Split('/')[2] == "master")
            {
                Console.WriteLine($"Pushing with commit {requestBodyJObject["before"]}...");
                ProcessStartInfo processStartInfo = new ProcessStartInfo { FileName = "/bin/bash", Arguments = "/home/pi/Documents/govgameServer/deploy.sh", UseShellExecute = true };
                Process process = new Process { StartInfo = processStartInfo };
                process.Start();
                Environment.Exit(0);
            }

            return Ok();
        }
    }
}
