using GarageManagement.Domain.Entites.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageManagement.Infrastructure.DbContext
{
    public class RepairDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public RepairDbContext(DbContextOptions<RepairDbContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<VehicleBrand> Brands => Set<VehicleBrand>();
        public DbSet<VehicleModel> Models => Set<VehicleModel>();
        public DbSet<VehicleModelYear> ModelYears => Set<VehicleModelYear>();
        public DbSet<VehicleVariant> Variants => Set<VehicleVariant>();
        public DbSet<VehicleOwner> Owners => Set<VehicleOwner>();

        public DbSet<VehicleLookup> VehicleLookup => Set<VehicleLookup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Map entities to tables with schema first
            modelBuilder.Entity<Vehicle>().ToTable("vehicle", "vhc");
            modelBuilder.Entity<VehicleOwner>().ToTable("vehicleOwner", "vhc");
            modelBuilder.Entity<VehicleModel>().ToTable("VehicleModel", "vhc");
            modelBuilder.Entity<VehicleModelYear>().ToTable("vehicleModelYear", "vhc");
            modelBuilder.Entity<VehicleVariant>().ToTable("vehicleVariant", "vhc");
            modelBuilder.Entity<VehicleBrand>().ToTable("VehicleBrand", "vhc");
            modelBuilder.Entity<VehicleLookup>().ToTable("VehicleLookup", "vhc");



            // Declare alternate keys first
            modelBuilder.Entity<VehicleBrand>()
                .HasAlternateKey(b => b.BrandID);

            modelBuilder.Entity<VehicleModel>()
                .HasAlternateKey(m => m.ModelID);

            modelBuilder.Entity<VehicleModelYear>()
                .HasAlternateKey(my => my.ModelYearID);

            //modelBuilder.Entity<VehicleVariant>()
            //    .HasAlternateKey(v => v.VariantID);

            //modelBuilder.Entity<Vehicle>()
            //    .HasAlternateKey(v => v.VehicleID);
            modelBuilder.Entity<Vehicle>()
    .HasIndex(v => v.VehicleID)
    .IsUnique();


            // VehicleModelYear -> VehicleModel
            modelBuilder.Entity<VehicleModelYear>()
                .HasOne(vmy => vmy.Model)
                .WithMany(vm => vm.ModelYears)
                .HasForeignKey(vmy => vmy.ModelID)
                .HasPrincipalKey(vm => vm.ModelID);

            // Vehicle -> VehicleBrand
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandID)
                .HasPrincipalKey(b => b.BrandID);

            // Vehicle -> VehicleModel
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.ModelID)
                .HasPrincipalKey(vm => vm.ModelID);

            // Vehicle -> VehicleModelYear
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.ModelYear)
                .WithMany(my => my.Vehicles)
                .HasForeignKey(v => v.ModelYearID)
                .HasPrincipalKey(my => my.ModelYearID);

            // VehicleModel -> VehicleBrand
            modelBuilder.Entity<VehicleModel>()
                .HasOne(vm => vm.Brand)
                .WithMany(b => b.VehicleModels)
                .HasForeignKey(vm => vm.BrandID)
                .HasPrincipalKey(b => b.BrandID);


           

          

            modelBuilder.Entity<VehicleVariant>()
                .HasOne(vv => vv.ModelYear)
                .WithMany(my => my.Variants)
                .HasForeignKey(vv => vv.ModelYearID)
                .HasPrincipalKey(my => my.ModelYearID);

            // VehicleOwner -> Vehicle
            modelBuilder.Entity<VehicleOwner>()
                .HasOne(vo => vo.Vehicle)
                .WithMany(v => v.Owners)
                .HasForeignKey(vo => vo.VehicleID)
                .HasPrincipalKey(v => v.VehicleID);

                modelBuilder.Entity<VehicleLookup>()
                    .HasIndex(v => new { v.LookupType, v.LookupValue })
                    .IsUnique()
                    .HasDatabaseName("UQ_LookupType_Value");

            modelBuilder.Entity<VehicleLookup>()
                .Property(v => v.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<VehicleLookup>()
                .Property(v => v.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<VehicleLookup>()
                .Property(v => v.CreatedAt)
                .HasDefaultValueSql("getdate()");



        }
    }

}
