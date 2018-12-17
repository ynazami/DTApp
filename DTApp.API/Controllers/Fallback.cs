using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DTApp.API.Controllers
{
    public class Fallback: Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
            "wwwroot", "Index.html"), "text/HTML");
        }
    }
}