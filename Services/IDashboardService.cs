using GeoWorldHotelSearch.Models;

namespace GeoWorldHotelSearch.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task LogSearchQueryAsync(string query, string userIp, string? userLocation, int resultCount);
    Task<IEnumerable<SearchQuery>> GetRecentSearchesAsync(int count = 10);
    Task<Dictionary<string, int>> GetTopSearchTermsAsync(int count = 10);
    Task<Dictionary<string, int>> GetTopLocationsAsync(int count = 10);
    Task<Dictionary<string, int>> GetSearchesByDayAsync(int days = 7);
}