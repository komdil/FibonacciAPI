using FibonacciAPI.Middlewares;
using FibonacciAPI.Queries;
using FibonacciAPI.Services;
using FibonacciAPI.Utilities;
using FluentValidation;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<IValidator<GetSubsequenceQuery>, GetSubsequenceQueryValidator>();
services.AddScoped<IFibonacciSequenceService, FibonacciSequenceService>();

var serviceProvider = services.BuildServiceProvider();
var logger = serviceProvider.GetService<ILogger<ApplicationLog>>();
services.AddSingleton(typeof(ILogger), logger);

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
