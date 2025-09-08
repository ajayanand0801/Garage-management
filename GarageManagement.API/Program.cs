using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.Mapper;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Interfaces.Validator;
using GarageManagement.Application.Mappings;
using GarageManagement.Application.Services;
using GarageManagement.Application.Services.Quotations;
using GarageManagement.Application.Services.Request;
using GarageManagement.Infrastructure;
using GarageManagement.Infrastructure.Repositories;
using GarageManagement.Infrastructure.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Garage Management API",
        Version = "v1",
        Description = "API for managing garages, vehicles, owners, and service records."
    });

    // Include XML documentation for Swagger (optional but recommended)
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Repository registrations
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleLookupRepository, VehicleLookupRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IQuotationRepository, QuotationRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleLookupService, VehicleLookupService>();
builder.Services.AddScoped<IServiceRequest, ServiceRequestService>();
builder.Services.AddScoped<IQuotationService, QuotationService>();

// Validator
builder.Services.AddScoped<IJsonValidator, JsonValidator>();

// AutoMapper & Mapping utilities
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IMapperUtility, MapperUtility>();

// Add DbContext + Connection String
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

// Uncomment if circular references exist
// builder.Services.AddControllers().AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
// });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Garage Management API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root path
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
