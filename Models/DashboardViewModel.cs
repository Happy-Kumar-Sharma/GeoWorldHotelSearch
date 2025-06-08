namespace GeoWorldHotelSearch.Models;

public class DashboardViewModel
{
    public int TotalHotels { get; set; }
    public int TotalSearches { get; set; }
    public List<SearchQuery> RecentSearches { get; set; } = new();
    public Dictionary<string, int> TopSearchTerms { get; set; } = new();
    public Dictionary<string, int> TopLocations { get; set; } = new();
    public Dictionary<string, int> SearchesByDay { get; set; } = new();
}