using Microsoft.AspNetCore.Mvc;
using GeoWorldHotelSearch.Models;
using GeoWorldHotelSearch.Services;

namespace GeoWorldHotelSearch.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<ApiController> _logger;

    public ApiController(
        IHotelService hotelService,
        IDashboardService dashboardService,
        ILogger<ApiController> logger)
    {
        _hotelService = hotelService;
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("hotels")]
    public async Task<IActionResult> GetHotels(int page = 1, int pageSize = 10)
    {
        var hotels = await _hotelService.GetAllHotelsAsync(page, pageSize);
        var totalCount = await _hotelService.GetTotalHotelsCountAsync();
        
        return Ok(new { 
            data = hotels, 
            total = totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        });
    }

    [HttpGet("hotels/{id}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }
        
        return Ok(hotel);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query, int page = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query cannot be empty");
        }
        
        var searchResult = await _hotelService.SearchHotelsAsync(query, page, pageSize);
        
        // Log the search query
        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        await _dashboardService.LogSearchQueryAsync(query, userIp, null, searchResult.Hotels.Count());
        
        return Ok(new { 
            data = searchResult.Hotels, 
            total = searchResult.Total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling((double)searchResult.Total / pageSize)
        });
    }

    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel(Hotel hotel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var createdHotel = await _hotelService.CreateHotelAsync(hotel);
        return CreatedAtAction(nameof(GetHotel), new { id = createdHotel.Id }, createdHotel);
    }

    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(int id, Hotel hotel)
    {
        if (id != hotel.Id)
        {
            return BadRequest("ID mismatch");
        }
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var existingHotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (existingHotel == null)
        {
            return NotFound();
        }
        
        await _hotelService.UpdateHotelAsync(hotel);
        return NoContent();
    }

    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        
        if (hotel == null)
        {
            return NotFound();
        }
        
        await _hotelService.DeleteHotelAsync(id);
        return NoContent();
    }

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync();
        return Ok(dashboardData);
    }
}