using System.Reflection;
using eServiceOnline.Models.Commons;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eServiceOnline.Controllers
{
    public class ResourceBoardController : eServicePageController
    {
        // GET
        public IActionResult Index(string selectedDistricts = null)
        {          
            this.ViewBag.HighLight = "Resource";
            ViewBag.VersionNumber = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            SetDistrictSelection(selectedDistricts);

            return View();
        }
    }
}