using GeoWorldHotelSearch.Models;
using Nest;

namespace GeoWorldHotelSearch.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticsearchService> _logger;
    private const string IndexName = "hotels";

    public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task<bool> IndexExistsAsync()
    {
        var exists = await _elasticClient.Indices.ExistsAsync(IndexName);
        return exists.Exists;
    }

    public async Task CreateIndexAsync()
    {
        if (await IndexExistsAsync())
            return;

        var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
            .Settings(s => s
                .Analysis(a => a
                    .Analyzers(an => an
                        .Custom("hotel_analyzer", ca => ca
                            .Tokenizer("standard")
                            .Filters("lowercase", "asciifolding", "stop", "snowball")
                        )
                    )
                )
                .Setting("max_result_window", 50000)
            )
            .Map<Hotel>(m => m
                .Properties(p => p
                    .Text(t => t
                        .Name(n => n.Name)
                        .Analyzer("hotel_analyzer")
                        .Fields(f => f
                            .Keyword(k => k.Name("keyword"))
                        )
                    )
                    .Text(t => t
                        .Name(n => n.Description)
                        .Analyzer("hotel_analyzer")
                    )
                    .Text(t => t
                        .Name(n => n.Location)
                        .Analyzer("hotel_analyzer")
                        .Fields(f => f
                            .Keyword(k => k.Name("keyword"))
                        )
                    )
                    .Text(t => t
                        .Name(n => n.City)
                        .Analyzer("hotel_analyzer")
                        .Fields(f => f
                            .Keyword(k => k.Name("keyword"))
                        )
                    )
                    .Text(t => t
                        .Name(n => n.Country)
                        .Analyzer("hotel_analyzer")
                        .Fields(f => f
                            .Keyword(k => k.Name("keyword"))
                        )
                    )
                    .GeoPoint(g => g
                        .Name("location")
                        // .LatLon()
                    )
                    .Number(n => n
                        .Name(h => h.PricePerNight)
                        .Type(NumberType.Double)
                    )
                    .Number(n => n
                        .Name(h => h.Rating)
                        .Type(NumberType.Double)
                    )
                    .Number(n => n
                        .Name(h => h.Stars)
                        .Type(NumberType.Integer)
                    )
                )
            )
        );

        if (!createIndexResponse.IsValid)
        {
            _logger.LogError("Failed to create index: {ErrorMessage}", createIndexResponse.DebugInformation);
            throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
        }
    }

    public async Task IndexHotelAsync(Hotel hotel)
    {
        var response = await _elasticClient.IndexDocumentAsync(hotel);
        
        if (!response.IsValid)
        {
            _logger.LogError("Failed to index hotel: {ErrorMessage}", response.DebugInformation);
            throw new Exception($"Failed to index hotel: {response.DebugInformation}");
        }
    }

    public async Task BulkIndexHotelsAsync(IEnumerable<Hotel> hotels)
    {
        var bulkResponse = await _elasticClient.BulkAsync(b => b
            .Index(IndexName)
            .IndexMany(hotels)
        );

        if (bulkResponse.Errors)
        {
            _logger.LogError("Failed to bulk index hotels: {ErrorMessage}", bulkResponse.DebugInformation);
            throw new Exception($"Failed to bulk index hotels: {bulkResponse.DebugInformation}");
        }
    }

    public async Task<(IEnumerable<Hotel> Hotels, long Total)> SearchAsync(string query, int page = 1, int pageSize = 10)
    {
        var searchResponse = await _elasticClient.SearchAsync<Hotel>(s => s
            .Index(IndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(f => f
                        .Field(h => h.Name, 3.0)
                        .Field(h => h.Location, 2.0)
                        .Field(h => h.City, 2.0)
                        .Field(h => h.Description, 1.0)
                    )
                    .Query(query)
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(Fuzziness.Auto)
                )
            )
            .Sort(sort => sort
                .Descending(SortSpecialField.Score)
            )
        );

        if (!searchResponse.IsValid)
        {
            _logger.LogError("Failed to search hotels: {ErrorMessage}", searchResponse.DebugInformation);
            throw new Exception($"Failed to search hotels: {searchResponse.DebugInformation}");
        }

        return (searchResponse.Documents, searchResponse.Total);
    }

    public async Task DeleteHotelFromIndexAsync(int id)
    {
        var response = await _elasticClient.DeleteAsync<Hotel>(id, d => d.Index(IndexName));
        
        if (!response.IsValid && response.Result != Result.NotFound)
        {
            _logger.LogError("Failed to delete hotel from index: {ErrorMessage}", response.DebugInformation);
            throw new Exception($"Failed to delete hotel from index: {response.DebugInformation}");
        }
    }

    public async Task UpdateHotelInIndexAsync(Hotel hotel)
    {
        await IndexHotelAsync(hotel);
    }

    public async Task ReindexAllAsync()
    {
        // This would typically be called from a background service or admin action
        // Implementation would depend on how you retrieve all hotels from the database
        throw new NotImplementedException("This method should be implemented based on your specific requirements");
    }
}