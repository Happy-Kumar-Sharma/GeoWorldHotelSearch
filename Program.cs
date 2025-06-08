using GeoWorldHotelSearch.Data;
using GeoWorldHotelSearch.Services;
using GeoWorldHotelSearch.Repositories;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure PostgreSQL with EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Elasticsearch
var elasticUri = new Uri(builder.Configuration["Elasticsearch:Url"]);
var settings = new ConnectionSettings(elasticUri)
    .DefaultIndex("hotels")
    .BasicAuthentication("elastic", "mftFM7o7") // 👈 set your real credentials
    .EnableDebugMode();

var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

// Register repositories and services
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "GeoWorldHotelSearch API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
