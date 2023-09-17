using Packt.Shared;
using Microsoft.AspNetCore.Mvc.Formatters;
using static System.Console;
using Northwind.WebApi.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.HttpLogging;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5002/");

// Add services to the container.

builder.Services.AddControllers(options =>
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter == null)
        {
            WriteLine($" {formatter.GetType().Name}");
        }
        else // класс форматера вывода с поддерживаемыми медиаформатами
        {
            WriteLine(" {0}, Media types: {1}",
            arg0: mediaFormatter.GetType().Name,
            arg1: string.Join(", ",
            mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddNorthwindContext();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // по умолчанию 32 Кбайт
    options.ResponseBodyLogLimit = 4096; // по умолчанию 32 Кбайт
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    { Title = "Northwind Service API", Version = "v1" });
});

builder.Services.AddCors();

builder.Services.AddScoped<IProductRepository , ProductRepository>();

var app = builder.Build();

app.UseCors(configurePolicy: options =>
{
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    options.WithOrigins("https://localhost:5001"); // допускаются запросы от клиента MVC
});

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[] {
        SubmitMethod.Get, SubmitMethod.Post,
        SubmitMethod.Put, SubmitMethod.Delete });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
