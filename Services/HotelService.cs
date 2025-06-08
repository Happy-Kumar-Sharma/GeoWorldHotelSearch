using Bogus;
using GeoWorldHotelSearch.Models;
using GeoWorldHotelSearch.Repositories;

namespace GeoWorldHotelSearch.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<HotelService> _logger;

    public HotelService(
        IHotelRepository hotelRepository,
        IElasticsearchService elasticsearchService,
        ILogger<HotelService> logger)
    {
        _hotelRepository = hotelRepository;
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<IEnumerable<Hotel>> GetAllHotelsAsync(int page = 1, int pageSize = 10)
    {
        return await _hotelRepository.GetAllAsync(page, pageSize);
    }

    public async Task<Hotel?> GetHotelByIdAsync(int id)
    {
        return await _hotelRepository.GetByIdAsync(id);
    }

    public async Task<int> GetTotalHotelsCountAsync()
    {
        return await _hotelRepository.GetTotalCountAsync();
    }

    public async Task<Hotel> CreateHotelAsync(Hotel hotel)
    {
        var createdHotel = await _hotelRepository.AddAsync(hotel);
        await _elasticsearchService.IndexHotelAsync(createdHotel);
        return createdHotel;
    }

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        await _hotelRepository.UpdateAsync(hotel);
        await _elasticsearchService.UpdateHotelInIndexAsync(hotel);
    }

    public async Task DeleteHotelAsync(int id)
    {
        await _hotelRepository.DeleteAsync(id);
        await _elasticsearchService.DeleteHotelFromIndexAsync(id);
    }

    public async Task<(IEnumerable<Hotel> Hotels, long Total)> SearchHotelsAsync(string query, int page = 1, int pageSize = 10)
    {
        return await _elasticsearchService.SearchAsync(query, page, pageSize);
    }

    public async Task SeedHotelsAsync(int count = 1000)
    {
        _logger.LogInformation("Starting to seed {Count} hotels", count);

        // Check if Elasticsearch index exists, create if not
        if (!await _elasticsearchService.IndexExistsAsync())
        {
            await _elasticsearchService.CreateIndexAsync();
        }

        // Generate fake hotel data
        var faker = new Faker<Hotel>()
            .RuleFor(h => h.Name, f => f.Company.CompanyName() + " Hotel")
            .RuleFor(h => h.Description, f => f.Lorem.Paragraphs(3))
            .RuleFor(h => h.Location, f => f.Address.StreetAddress())
            .RuleFor(h => h.City, f => f.Address.City())
            .RuleFor(h => h.Country, f => f.Address.Country())
            .RuleFor(h => h.Latitude, f => f.Address.Latitude())
            .RuleFor(h => h.Longitude, f => f.Address.Longitude())
            .RuleFor(h => h.PricePerNight, f => f.Random.Decimal(50, 1000))
            .RuleFor(h => h.Rating, f => f.Random.Double(1, 5))
            .RuleFor(h => h.Stars, f => f.Random.Int(1, 5))
            .RuleFor(h => h.Amenities, f => f.Make(f.Random.Int(3, 10), () => f.Commerce.ProductAdjective()).ToList())
            .RuleFor(h => h.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(h => h.CreatedAt, f => DateTime.SpecifyKind(f.Date.Past(2), DateTimeKind.Utc))
            .RuleFor(h => h.UpdatedAt, (f, h) => h.CreatedAt);

        var hotels = faker.Generate(count);
        
        // Process in batches to avoid overwhelming the database
        const int batchSize = 10000;
        for (int i = 0; i < count; i += batchSize)
        {
            var batch = hotels.Skip(i).Take(batchSize).ToList();

            // Add to database
            // foreach (var hotel in batch)
            // {
            //     await _hotelRepository.AddAsync(hotel);
            // }
            // Batch insert
            await _hotelRepository.AddRangeAsync(batch);
            
            // Index in Elasticsearch
            await _elasticsearchService.BulkIndexHotelsAsync(batch);
            
            _logger.LogInformation("Seeded batch {BatchNumber} of {TotalBatches}", 
                (i / batchSize) + 1, Math.Ceiling((double)count / batchSize));
        }

        _logger.LogInformation("Completed seeding {Count} hotels", count);
    }
}