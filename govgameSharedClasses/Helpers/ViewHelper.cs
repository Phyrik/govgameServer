using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace govgameSharedClasses.Helpers
{
    public class ViewHelper
    {
        public static ViewResult MinistryDashboardView(Controller controller, string rootPath, MinistryHelper.MinistryCode ministryCode, string page)
        {
            string ministryFolderName = "";
            switch (ministryCode)
            {
                case MinistryHelper.MinistryCode.None:
                    return null;

                case MinistryHelper.MinistryCode.PrimeMinister:
                    ministryFolderName = "PrimeMinisterDashboard";
                    break;

                case MinistryHelper.MinistryCode.Interior:
                    break;

                case MinistryHelper.MinistryCode.FinanceAndTrade:
                    ministryFolderName = "FinanceAndTradeDashboard";
                    break;

                case MinistryHelper.MinistryCode.ForeignAffairs:
                    break;

                case MinistryHelper.MinistryCode.Defence:
                    break;
            }

            if (File.Exists($@"{rootPath}/Views/Game/{ministryFolderName}/{page}.cshtml"))
            {
                return controller.View($"./{ministryFolderName}/{page}");
            }
            else
            {
                return controller.View("404");
            }
        }
    }
}
