using ChainResource;
using ChainResource.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

IConfigurationSection storageServicesSection = configuration.GetSection("StorageServices");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var chainResource = StorageFactory.GetChainResource<ExchangeRateList>(storageServicesSection);

app.MapGet("/Rates", () => chainResource.GetValue())
.WithOpenApi();

app.Run();