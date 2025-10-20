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
            var allServices = _dbContext.Services
                .Include(s => s.Category)
                .AsNoTracking()
                .OrderBy(s => s.Category.CategoryName)
                .ThenBy(s => s.ServiceName)
                .ToList();

            var viewModel = new ContactUsViewModel
            {
                AllServices = allServices
            };

            return View(viewModel);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> DebugDatabase()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                var formCount = await _dbContext.FormSubmissions.CountAsync();
                var serviceCount = await _dbContext.Services.CountAsync();
                var categoryCount = await _dbContext.Categories.CountAsync();

                ViewBag.CanConnect = canConnect;
                ViewBag.FormCount = formCount;
                ViewBag.ServiceCount = serviceCount;
                ViewBag.CategoryCount = categoryCount;

                // Get recent form submissions
                var recentSubmissions = await _dbContext.FormSubmissions
                    .Include(f => f.Service)
                    .OrderByDescending(f => f.SubmissionDate)
                    .Take(5)
                    .ToListAsync();

                ViewBag.RecentSubmissions = recentSubmissions;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult TestForm()
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
