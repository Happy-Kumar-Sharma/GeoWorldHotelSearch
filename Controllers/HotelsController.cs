using Microsoft.AspNetCore.Mvc;
using GeoWorldHotelSearch.Models;
using GeoWorldHotelSearch.Services;

namespace GeoWorldHotelSearch.Controllers;

public class HotelsController : Controller
{
    private readonly IHotelService _hotelService;
    private readonly IDashboardService _dashboardService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HotelsController> _logger;
    private readonly IElasticsearchService _elasticsearchService;

    public HotelsController(
        IHotelService hotelService,
        IDashboardService dashboardService,
        IConfiguration configuration,
        ILogger<HotelsController> logger,
        IElasticsearchService elasticsearchService)
    {
        _hotelService = hotelService;
        _dashboardService = dashboardService;
        _configuration = configuration;
        _logger = logger;
        _elasticsearchService = elasticsearchService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var pageSize = _configuration.GetValue<int>("Pagination:PageSize", 10);
        var hotels = await _hotelService.GetAllHotelsAsync(page, pageSize);
        var totalHotels = await _hotelService.GetTotalHotelsCountAsync();
        
        var viewModel = new SearchViewModel
        {
            Results = hotels.ToList(),
            TotalResults = totalHotels,
            CurrentPage = page,
            PageSize = pageSize
        };
        
        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }
        
        // Get latest booking for this hotel (for demo: show most recent by CreatedAt)
        Booking? latestBooking = null;
        using (var scope = HttpContext.RequestServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<GeoWorldHotelSearch.Data.AppDbContext>();
            latestBooking = db?.Bookings
                .Where(b => b.HotelId == id)
                .OrderByDescending(b => b.CreatedAt)
                .FirstOrDefault();
        }
        
        return View((hotel, latestBooking));
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query, int page = 1, 
        decimal? minPrice = null, decimal? maxPrice = null, 
        List<int>? stars = null, List<string>? amenities = null,
        string sortOrder = "relevance")
    {
        int pageSize = _configuration.GetValue<int>("Pagination:PageSize", 10);
        var searchResults = await _hotelService.SearchHotelsAsync(query, page, pageSize, minPrice, maxPrice, stars, amenities, sortOrder);
        
        var viewModel = new SearchViewModel
        {
            Query = query,
            Results = searchResults.Hotels.ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalResults = (int)searchResults.Total
        };
        
        ViewBag.Query = query;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.Stars = stars;
        ViewBag.Amenities = amenities;
        ViewBag.SortOrder = sortOrder;
        
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Hotel hotel)
    {
        if (ModelState.IsValid)
        {
            // ✅ Ensure UTC
            hotel.CreatedAt = DateTime.UtcNow;
            hotel.UpdatedAt = DateTime.UtcNow;
            await _hotelService.CreateHotelAsync(hotel);
            return RedirectToAction(nameof(Index));
        }
        return View(hotel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }
        
        return View(hotel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Hotel hotel)
    {
        if (id != hotel.Id)
        {
            return NotFound();
        }

        // Bind amenities from form
        if (Request.Form.TryGetValue("AmenitiesList", out var amenities))
        {
            hotel.Amenities = amenities.ToList();
        }
        else
        {
            hotel.Amenities = new List<string>();
        }

        // Ensure UTC for CreatedAt and UpdatedAt
        hotel.CreatedAt = DateTime.SpecifyKind(hotel.CreatedAt, DateTimeKind.Utc);
        hotel.UpdatedAt = DateTime.UtcNow;

        if (ModelState.IsValid)
        {
            await _hotelService.UpdateHotelAsync(hotel);
            return RedirectToAction(nameof(Index));
        }
        return View(hotel);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }
        
        return View(hotel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _hotelService.DeleteHotelAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SeedData(int count = 1000)
    {
        await _hotelService.SeedHotelsAsync(count);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetFeatured(int id, bool isFeatured, string? returnUrl = null)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }
        hotel.IsFeatured = isFeatured;
        hotel.UpdatedAt = DateTime.UtcNow;
        await _hotelService.UpdateHotelAsync(hotel);
        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }
}
