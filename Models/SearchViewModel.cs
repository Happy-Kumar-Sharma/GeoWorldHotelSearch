using System.Collections.Generic;

namespace GeoWorldHotelSearch.Models
{
    public class SearchViewModel
    {
        public string Query { get; set; }
        public List<Hotel> Results { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages => (TotalResults + PageSize - 1) / PageSize;
    }
}
