using System.Diagnostics;
using DragoTactical.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DragoTacticalDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, DragoTacticalDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CyberServices()
        {
            // CategoryId = 2 for Cyber services per provided data
            var cyberServices = _dbContext.Services
                .AsNoTracking()
                .Where(s => s.CategoryId == 2)
                .OrderBy(s => s.ServiceName)
                .ToList();

            return View(cyberServices);
        }

        public IActionResult PhysicalServices()
        {
            var physicalServices = _dbContext.Services
                .AsNoTracking()
                .Where(s => s.CategoryId == 1)
                .OrderBy(s => s.ServiceName)
                .ToList();

            return View(physicalServices);
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
