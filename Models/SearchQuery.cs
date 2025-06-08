namespace GeoWorldHotelSearch.Models;

public class SearchQuery
{
    public int Id { get; set; }
    public string Query { get; set; } = string.Empty;
    public string UserIp { get; set; } = string.Empty;
    public string? UserLocation { get; set; }
    public int ResultCount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}