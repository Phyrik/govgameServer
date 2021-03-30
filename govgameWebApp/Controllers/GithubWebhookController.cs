using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace govgameWebApp.Controllers
{
    public class GithubWebhookController : Controller
    {
        public IActionResult Index()
        {
            JObject requestObject = JObject.Parse(new StreamReader(Request.Body).ReadToEndAsync().Result);

            if (requestObject["repository"]["id"].ToString() == "328969055")
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = @"-NoProfile -ExecutionPolicy unrestricted -File ""C:\Users\Administrator\Documents\deploy.ps1""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                string output = string.Empty;
                Process process = Process.Start(processStartInfo);
                output += Environment.CurrentDirectory + " [NEW LINE] ";
                /*int i = 0;
                while ((!process.StandardOutput.EndOfStream && !process.StandardError.EndOfStream) && i < 2)
                {*/
                    string line = process.StandardOutput.ReadLine();
                    output += line + " [NEW LINE] ";
                    line = process.StandardError.ReadLine();
                    output += line + " [NEW LINE] ";
                    /*i++;
                }*/

                return Content($"received github webhook successfully! output: {output}");
            }

            return Content($"Failure: {requestObject["repository"]["id"].ToString()} == {Environment.GetEnvironmentVariable("govgameServerRepoId").ToString()}");
        }
    }
}
