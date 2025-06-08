using GeoWorldHotelSearch.Models;

namespace GeoWorldHotelSearch.Services;

public interface IHotelService
{
    Task<IEnumerable<Hotel>> GetAllHotelsAsync(int page = 1, int pageSize = 10);
    Task<Hotel?> GetHotelByIdAsync(int id);
    Task<int> GetTotalHotelsCountAsync();
    Task<Hotel> CreateHotelAsync(Hotel hotel);
    Task UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelAsync(int id);
    Task<(IEnumerable<Hotel> Hotels, long Total)> SearchHotelsAsync(string query, int page = 1, int pageSize = 10);
    Task<(IEnumerable<Hotel> Hotels, long Total)> SearchHotelsAsync(string? query, int page = 1, int pageSize = 10, decimal? minPrice = null, decimal? maxPrice = null, List<int>? stars = null, List<string>? amenities = null, string sortOrder = "relevance");
    Task SeedHotelsAsync(int count = 1000);
    Task<IEnumerable<Hotel>> GetFeaturedHotelsAsync(int maxCount = 6);
}