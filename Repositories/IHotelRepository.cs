using GeoWorldHotelSearch.Models;

namespace GeoWorldHotelSearch.Repositories;

public interface IHotelRepository
{
    Task<IEnumerable<Hotel>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<Hotel?> GetByIdAsync(int id);
    Task<int> GetTotalCountAsync();
    Task<Hotel> AddAsync(Hotel hotel);
    Task AddRangeAsync(IEnumerable<Hotel> hotels);
    Task UpdateAsync(Hotel hotel);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}