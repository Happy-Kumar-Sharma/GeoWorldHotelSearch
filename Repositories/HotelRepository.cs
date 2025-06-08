using GeoWorldHotelSearch.Data;
using GeoWorldHotelSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoWorldHotelSearch.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hotel>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        return await _context.Hotels
            .OrderBy(h => h.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Hotel?> GetByIdAsync(int id)
    {
        return await _context.Hotels.FindAsync(id);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Hotels.CountAsync();
    }

    public async Task<Hotel> AddAsync(Hotel hotel)
    {
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return hotel;
    }

    public async Task AddRangeAsync(IEnumerable<Hotel> hotels)
    {
        _context.Hotels.AddRange(hotels);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Hotel hotel)
    {
        _context.Entry(hotel).State = EntityState.Modified;
        hotel.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Hotels.AnyAsync(h => h.Id == id);
    }

    public async Task<IEnumerable<Hotel>> GetFeaturedAsync(int maxCount = 6)
    {
        return await _context.Hotels
            .Where(h => h.IsFeatured)
            .OrderByDescending(h => h.UpdatedAt)
            .Take(maxCount)
            .ToListAsync();
    }
}