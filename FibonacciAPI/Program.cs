using FibonacciAPI.Middlewares;
using FibonacciAPI.Services.CacheService;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<IValidator<GetSubsequenceQuery>, GetSubsequenceQueryValidator>();
services.AddScoped<IFibonacciSequenceService, FibonacciSequenceService>();
services.AddScoped<IFibonacciPositionGeneratorService, FibonacciPositionGeneratorService>();
services.AddScoped<IFibonacciNextNumberGeneratorService, FibonacciNextNumberGeneratorService>();
services.AddScoped<IFibonacciCacheService, FibonacciCacheService>();
services.AddLogging();
services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();
