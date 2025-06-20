using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GeoWorldHotelSearch.Models;
using GeoWorldHotelSearch.Services;
using System.IO;

namespace GeoWorldHotelSearch.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHotelService _hotelService;
    private readonly IDashboardService _dashboardService;

    public HomeController(
        ILogger<HomeController> logger,
        IHotelService hotelService,
        IDashboardService dashboardService)
    {
        _logger = logger;
        _hotelService = hotelService;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        // Show only featured hotels on the home page
        var featuredHotels = await _hotelService.GetFeaturedHotelsAsync(6);
        return View(featuredHotels);
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
    
    public IActionResult Documentation()
    {
        // Serve the static HTML documentation as the documentation page
        var docPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Home", "ProjectDocumentation.html");
        var html = System.IO.File.ReadAllText(docPath);
        return Content(html, "text/html");
    }
}
