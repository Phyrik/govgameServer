using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace govgameWebApp.Controllers
{
    public class GithubWebhookController : Controller
    {
        public IActionResult Index()
        {
            JObject requestObject = JObject.Parse(new StreamReader(Request.Body).ReadToEndAsync().Result);

            if (requestObject["repository"]["id"].ToString() == Environment.GetEnvironmentVariable("govgameServerRepoId").ToString())
            {
                Runspace runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.Add(@"C:\Users\Administrator\Documents\deploy.ps1");
                pipeline.Invoke();
                runspace.Close();

                return Content($"received github webhook successfully!");
            }

            return Unauthorized($"{requestObject["repository"]["id"].ToString()} == {Environment.GetEnvironmentVariable("govgameServerRepoId").ToString()}");
        }
    }
}
