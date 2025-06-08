using GeoWorldHotelSearch.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GeoWorldHotelSearch.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<SearchQuery> SearchQueries { get; set; }
    public DbSet<Booking> Bookings { get; set; } // Added Bookings DbSet

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Hotel entity
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PricePerNight).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Rating).HasColumnType("decimal(3,1)");
            
            // Store Amenities as JSON
            entity.Property(e => e.Amenities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>());
        });

        // Configure SearchQuery entity
        modelBuilder.Entity<SearchQuery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Query).IsRequired();
            entity.Property(e => e.UserIp).IsRequired();
        });

        // Configure Booking entity
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HotelId).IsRequired();
            entity.Property(e => e.HotelName).IsRequired();
            entity.Property(e => e.CheckIn).IsRequired();
            entity.Property(e => e.CheckOut).IsRequired();
            entity.Property(e => e.Guests).IsRequired();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            // Remove UserId, BookingDate, Status as they do not exist in Booking model
        });
    }
}

// protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//     var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
//         v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
//         v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
//     );

//     foreach (var entityType in modelBuilder.Model.GetEntityTypes())
//     {
//         foreach (var property in entityType.GetProperties())
//         {
//             if (property.ClrType == typeof(DateTime))
//             {
//                 property.SetValueConverter(dateTimeConverter);
//             }
//         }
//     }

//     base.OnModelCreating(modelBuilder);
// }
