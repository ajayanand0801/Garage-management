using GarageManagement.Application.Interfaces;
using GarageManagement.Application.Interfaces.ServiceInterface;
using GarageManagement.Application.Services;
using GarageManagement.Infrastructure.DbContext;
using GarageManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<RepairDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVehicleService, VehicleService>();

            return services;
        }
    }

}
