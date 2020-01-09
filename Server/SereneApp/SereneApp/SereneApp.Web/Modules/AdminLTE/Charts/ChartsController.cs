﻿
namespace SereneApp.AdminLTE
{
    using Serenity.Web;
    using Microsoft.AspNetCore.Mvc;

    [PageAuthorize, Route("AdminLTE/Charts/[action]")]
    public class ChartsController : Controller
    {
        public ActionResult ChartJS()
        {
            return View(MVC.Views.AdminLTE.Charts.ChartJS);
        }

        public ActionResult Flot()
        {
            return View(MVC.Views.AdminLTE.Charts.Flot);
        }

        public ActionResult InlineCharts()
        {
            return View(MVC.Views.AdminLTE.Charts.InlineCharts);
        }

        public ActionResult Morris()
        {
            return View(MVC.Views.AdminLTE.Charts.Morris);
        }
    }
}