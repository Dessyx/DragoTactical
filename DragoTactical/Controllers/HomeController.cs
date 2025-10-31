using System.Diagnostics;
using DragoTactical.Models;        // imports
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DragoTactical.Controllers
{
    //------------------------------------------------------------------------------------------------------
    // Home Controller - Handles main page views and navigation
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DragoTacticalDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, DragoTacticalDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        //------------------------------------------------------------------------------------------------------
        // Home page
        public IActionResult Index()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        // Cyber services page
        public IActionResult CyberServices()
        {
            var cyberServices = _dbContext.Services
                .AsNoTracking()
                .Where(s => s.CategoryId == 2)
                .OrderBy(s => s.ServiceName)
                .ToList();

            return View(cyberServices);
        }

        //------------------------------------------------------------------------------------------------------
        // Physical services page
        public IActionResult PhysicalServices()
        {
            var physicalServices = _dbContext.Services
                .AsNoTracking()
                .Where(s => s.CategoryId == 1)
                .OrderBy(s => s.ServiceName)
                .ToList();

            return View(physicalServices);
        }

        //------------------------------------------------------------------------------------------------------
        // Contact us page with services dropdown
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

        //------------------------------------------------------------------------------------------------------
        // About us page
        public IActionResult AboutUs()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        // Privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        // Debug database connection and data
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

        //------------------------------------------------------------------------------------------------------
        // Test form page
        public IActionResult TestForm()
        {
            return View();
        }

        //------------------------------------------------------------------------------------------------------
        // Error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
