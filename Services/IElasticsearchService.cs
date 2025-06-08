using GeoWorldHotelSearch.Models;

namespace GeoWorldHotelSearch.Services;

public interface IElasticsearchService
{
    Task CreateIndexAsync();
    Task IndexHotelAsync(Hotel hotel);
    Task BulkIndexHotelsAsync(IEnumerable<Hotel> hotels);
    Task<(IEnumerable<Hotel> Hotels, long Total)> SearchAsync(string query, int page = 1, int pageSize = 10);
    Task DeleteHotelFromIndexAsync(int id);
    Task UpdateHotelInIndexAsync(Hotel hotel);
    Task<bool> IndexExistsAsync();
    Task ReindexAllAsync();
}