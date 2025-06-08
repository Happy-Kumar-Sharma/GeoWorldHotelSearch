using GeoWorldHotelSearch.Data;
using GeoWorldHotelSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace GeoWorldHotelSearch.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly IHotelService _hotelService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        AppDbContext context,
        IHotelService hotelService,
        ILogger<DashboardService> logger)
    {
        _context = context;
        _hotelService = hotelService;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        var totalHotels = await _hotelService.GetTotalHotelsCountAsync();
        var totalSearches = await _context.SearchQueries.CountAsync();
        var recentSearches = await GetRecentSearchesAsync();
        var topSearchTerms = await GetTopSearchTermsAsync();
        var topLocations = await GetTopLocationsAsync();
        var searchesByDay = await GetSearchesByDayAsync();

        return new DashboardViewModel
        {
            TotalHotels = totalHotels,
            TotalSearches = totalSearches,
            RecentSearches = recentSearches.ToList(),
            TopSearchTerms = topSearchTerms,
            TopLocations = topLocations,
            SearchesByDay = searchesByDay
        };
    }

    public async Task LogSearchQueryAsync(string query, string userIp, string? userLocation, int resultCount)
    {
        var searchQuery = new SearchQuery
        {
            Query = query,
            UserIp = userIp,
            UserLocation = userLocation,
            ResultCount = resultCount,
            Timestamp = DateTime.UtcNow
        };

        _context.SearchQueries.Add(searchQuery);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SearchQuery>> GetRecentSearchesAsync(int count = 10)
    {
        return await _context.SearchQueries
            .OrderByDescending(q => q.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetTopSearchTermsAsync(int count = 10)
    {
        return await _context.SearchQueries
            .GroupBy(q => q.Query.ToLower())
            .Select(g => new { Term = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToDictionaryAsync(x => x.Term, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetTopLocationsAsync(int count = 10)
    {
        return await _context.SearchQueries
            .Where(q => q.UserLocation != null)
            .GroupBy(q => q.UserLocation!)
            .Select(g => new { Location = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToDictionaryAsync(x => x.Location, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetSearchesByDayAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);
        
        var searchesByDay = await _context.SearchQueries
            .Where(q => q.Timestamp >= startDate)
            .GroupBy(q => q.Timestamp.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToDictionaryAsync(x => x.Date.ToString("yyyy-MM-dd"), x => x.Count);

        // Fill in missing days with zero counts
        for (int i = 0; i < days; i++)
        {
            var date = startDate.AddDays(i).ToString("yyyy-MM-dd");
            if (!searchesByDay.ContainsKey(date))
            {
                searchesByDay[date] = 0;
            }
        }

        return searchesByDay;
    }
}