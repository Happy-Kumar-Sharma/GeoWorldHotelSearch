using GeoWorldHotelSearch.Data;
using GeoWorldHotelSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoWorldHotelSearch.Controllers;

public class BookingController : Controller
{
    private readonly AppDbContext _context;
    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int hotelId, string hotelName, DateTime checkIn, DateTime checkOut, int guests, decimal pricePerNight)
    {
        // Validation: Check-in must be before Check-out
        if (checkIn >= checkOut)
        {
            TempData["BookingError"] = "Check-out date must be after check-in date.";
            return RedirectToAction("Details", "Hotels", new { id = hotelId });
        }
        var totalNights = (checkOut - checkIn).Days;
        if (totalNights <= 0) totalNights = 1;
        var totalPrice = pricePerNight * totalNights * guests;
        var booking = new Booking
        {
            HotelId = hotelId,
            HotelName = hotelName,
            CheckIn = checkIn.ToUniversalTime(),
            CheckOut = checkOut.ToUniversalTime(),
            Guests = guests,
            TotalPrice = totalPrice,
            CreatedAt = DateTime.UtcNow
        };
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return RedirectToAction("Confirmation", new { id = booking.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Confirmation(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        return View(booking);
    }
}
