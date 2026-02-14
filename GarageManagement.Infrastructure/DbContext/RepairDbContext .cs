using GarageManagement.Domain.Entites;
using GarageManagement.Domain.Entites.Quotation;
using GarageManagement.Domain.Entites.Request;
using GarageManagement.Domain.Entites.Vehicles;
using GarageManagement.Domain.Entites.WorkOrder;
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

        //ServiceRequest 

        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<ServiceRequestDocument> ServiceRequestDocuments => Set<ServiceRequestDocument>();
        public DbSet<ServiceRequestMetadata> ServiceRequestMetadatas => Set<ServiceRequestMetadata>();
        public DbSet<ServiceRequestVehicleMetaData> ServiceRequestVehicleMetaData => Set<ServiceRequestVehicleMetaData>();
        public DbSet<ServiceRequestCustomerMetaData> ServiceRequestCustomerMetaData => Set<ServiceRequestCustomerMetaData>();

        //Quotation
        public DbSet<Quotation> Quotations=> Set<Quotation>();
        public DbSet<QuotationItem> QuotationItem => Set<QuotationItem>();

        //Customer
        public DbSet<Customer> Customers => Set<Customer>();

        //WorkOrder
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

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
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequest", "dbo");
            modelBuilder.Entity<ServiceRequestDocument>().ToTable("ServiceRequestDocument", "dbo");
            modelBuilder.Entity<ServiceRequestMetadata>().ToTable("ServiceRequestMetadata", "dbo");
            modelBuilder.Entity<ServiceRequestVehicleMetaData>().ToTable("SRVehicleMetaData", "dbo");
            modelBuilder.Entity<ServiceRequestCustomerMetaData>().ToTable("SRCustomerMetaData", "dbo");
            modelBuilder.Entity<Quotation>().ToTable("Quotation", "rpa");
            modelBuilder.Entity<QuotationItem>().ToTable("QuotationItem", "rpa");
            modelBuilder.Entity<Customer>().ToTable("Customer", "dbo");
            modelBuilder.Entity<WorkOrder>().ToTable("WorkOrder", "rpa");

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.Property(w => w.OrderGuid)
                    .IsRequired();
                entity.Property(w => w.Status)
                    .HasMaxLength(100)
                    .HasDefaultValue("Created");
            });

            modelBuilder.Entity<VehicleBrand>()
       .HasAlternateKey(b => b.BrandID);

            // VehicleModel: alternate key on ModelID
            modelBuilder.Entity<VehicleModel>()
                .HasAlternateKey(m => m.ModelID);

            // VehicleModelYear: alternate key on ModelYearID
            modelBuilder.Entity<VehicleModelYear>()
                .HasAlternateKey(my => my.ModelYearID);

            // (Optionally) VehicleVariant alternate key if you want a VariantID separate from PK
            // modelBuilder.Entity<VehicleVariant>()
            //     .HasAlternateKey(vv => vv.VariantID);

            // For Vehicle: you already have uniqueness on VehicleID
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.VehicleID)
                .IsUnique();

            // ========== RELATIONSHIPS ==========

            // VehicleModelYear → VehicleModel
            modelBuilder.Entity<VehicleModelYear>()
                .HasOne(vmy => vmy.Model)
                .WithMany(vm => vm.ModelYears)
                .HasForeignKey(vmy => vmy.ModelID)
                .HasPrincipalKey(vm => vm.ModelID);

            // Vehicle → VehicleBrand
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandID)
                .HasPrincipalKey(b => b.BrandID);

            // Vehicle → VehicleModel
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Model)
                .WithMany(vm => vm.Vehicles)
                .HasForeignKey(v => v.ModelID)
                .HasPrincipalKey(vm => vm.ModelID);

            // Vehicle → VehicleModelYear
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.ModelYear)
                .WithMany(my => my.Vehicles)
                .HasForeignKey(v => v.ModelYearID)
                .HasPrincipalKey(my => my.ModelYearID);

            // VehicleModel → VehicleBrand
            modelBuilder.Entity<VehicleModel>()
                .HasOne(vm => vm.Brand)
                .WithMany(b => b.VehicleModels)
                .HasForeignKey(vm => vm.BrandID)
                .HasPrincipalKey(b => b.BrandID);

            // VehicleVariant → VehicleModelYear
            modelBuilder.Entity<VehicleVariant>()
                .HasOne(vv => vv.ModelYear)
                .WithMany(my => my.Variants)
                .HasForeignKey(vv => vv.ModelYearID)
                .HasPrincipalKey(my => my.ModelYearID);

            // VehicleOwner → Vehicle
            modelBuilder.Entity<VehicleOwner>()
                .HasOne(vo => vo.Vehicle)
                .WithMany(v => v.Owners)
                .HasForeignKey(vo => vo.VehicleID)
                .HasPrincipalKey(v => v.VehicleID);

            //        // Declare alternate keys first
            //        modelBuilder.Entity<VehicleBrand>()
            //            .HasAlternateKey(b => b.BrandID);

            //        modelBuilder.Entity<VehicleModel>()
            //            .HasAlternateKey(m => m.ModelID);

            //        modelBuilder.Entity<VehicleModelYear>()
            //            .HasAlternateKey(my => my.ModelYearID);

            //        //modelBuilder.Entity<VehicleVariant>()
            //        //    .HasAlternateKey(v => v.VariantID);

            //        //modelBuilder.Entity<Vehicle>()
            //        //    .HasAlternateKey(v => v.VehicleID);
            //        modelBuilder.Entity<Vehicle>()
            //.HasIndex(v => v.VehicleID)
            //.IsUnique();


            //        // VehicleModelYear -> VehicleModel
            //        modelBuilder.Entity<VehicleModelYear>()
            //            .HasOne(vmy => vmy.Model)
            //            .WithMany(vm => vm.ModelYears)
            //            .HasForeignKey(vmy => vmy.ModelID)
            //            .HasPrincipalKey(vm => vm.ModelID);

            //        // Vehicle -> VehicleBrand
            //        modelBuilder.Entity<Vehicle>()
            //            .HasOne(v => v.Brand)
            //            .WithMany(b => b.Vehicles)
            //            .HasForeignKey(v => v.BrandID)
            //            .HasPrincipalKey(b => b.BrandID);

            //        // Vehicle -> VehicleModel
            //        modelBuilder.Entity<Vehicle>()
            //            .HasOne(v => v.Model)
            //            .WithMany(vm => vm.Vehicles)
            //            .HasForeignKey(v => v.ModelID)
            //            .HasPrincipalKey(vm => vm.ModelID);

            //        // Vehicle -> VehicleModelYear
            //        modelBuilder.Entity<Vehicle>()
            //            .HasOne(v => v.ModelYear)
            //            .WithMany(my => my.Vehicles)
            //            .HasForeignKey(v => v.ModelYearID)
            //            .HasPrincipalKey(my => my.ModelYearID);

            //        // VehicleModel -> VehicleBrand
            //        modelBuilder.Entity<VehicleModel>()
            //            .HasOne(vm => vm.Brand)
            //            .WithMany(b => b.VehicleModels)
            //            .HasForeignKey(vm => vm.BrandID)
            //            .HasPrincipalKey(b => b.BrandID);






            //        modelBuilder.Entity<VehicleVariant>()
            //            .HasOne(vv => vv.ModelYear)
            //            .WithMany(my => my.Variants)
            //            .HasForeignKey(vv => vv.ModelYearID)
            //            .HasPrincipalKey(my => my.ModelYearID);

            //        // VehicleOwner -> Vehicle
            //        modelBuilder.Entity<VehicleOwner>()
            //            .HasOne(vo => vo.Vehicle)
            //            .WithMany(v => v.Owners)
            //            .HasForeignKey(vo => vo.VehicleID)
            //            .HasPrincipalKey(v => v.VehicleID);

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


            //ServiceRequest 

            modelBuilder.Entity<ServiceRequest>()
        .HasMany(sr => sr.Documents)
        .WithOne(doc => doc.ServiceRequest)
        .HasForeignKey(doc => doc.RequestID);

            modelBuilder.Entity<ServiceRequest>()
                .HasMany(sr => sr.MetadataEntries)
                .WithOne(meta => meta.ServiceRequest)
                .HasForeignKey(meta => meta.RequestID);

            modelBuilder.Entity<ServiceRequest>()
  .HasMany(sr => sr.vehicleMetaData)
  .WithOne(doc => doc.ServiceRequest)
  .HasForeignKey(doc => doc.RequestID);

            modelBuilder.Entity<ServiceRequest>()
       .HasOne(sr => sr.customerMetaData)
       .WithOne(c => c.ServiceRequest)
       .HasForeignKey<ServiceRequestCustomerMetaData>(c => c.RequestID);
            // or Restrict, based on your logic


            //Quotaion 

            modelBuilder.Entity<Quotation>(entity =>
            {
               //entity.HasKey(q => q.QuotationId);

                entity.Property(q => q.QuoteGuid)
                      .HasDefaultValueSql("NEWID()");

                entity.Property(q => q.Status)
                      .HasMaxLength(50)
                      .HasDefaultValue("pending");

                entity.Property(q => q.CurrencyCode)
                      .HasColumnType("char(3)")
                      .IsRequired();

                // One-to-many relationship with QuotationItem
                //entity.HasMany(q => q.QuotationItems)
                //      .WithOne(qi => qi.Quotation)
                //      .HasForeignKey(qi => qi.QuotationID)
                //      .OnDelete(DeleteBehavior.Cascade);
            });

            // QuotationItem configuration
            modelBuilder.Entity<QuotationItem>(entity =>
            {
                //entity.HasKey(qi => qi.ItemGguid);

                entity.Property(qi => qi.Description)
                      .IsRequired();

                entity.Property(qi => qi.CurrencyCode)
                      .HasMaxLength(20);

                entity.Property(qi => qi.ItemType)
                      .HasMaxLength(50);

                entity.Property(qi => qi.Code)
                      .HasMaxLength(50);

                entity.Property(qi => qi.Quantity)
                      .IsRequired();

                entity.Property(qi => qi.NetTotal)
                      .HasComputedColumnSql("([TotalPrice] - ISNULL([DiscountAmount], 0) + ISNULL([TaxAmount], 0))", stored: true);

                entity.HasCheckConstraint("CK_QuotationItem_Quantity_Positive", "[Quantity] > 0");
            });

            //Customer configuration
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(c => c.CustomerGuid)
                      .HasDefaultValueSql("NEWID()");

                entity.Property(c => c.IsActive)
                      .HasDefaultValue(true);

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("sysutcdatetime()");

                entity.HasCheckConstraint("CK_Customer_CustomerType", "[CustomerType]='Company' OR [CustomerType]='Individual'");

                entity.HasCheckConstraint("CK_Customer_RequiredFields", 
                    "([CustomerType]='Individual' AND [FirstName] IS NOT NULL AND [LastName] IS NOT NULL) OR ([CustomerType]='Company' AND [CompanyName] IS NOT NULL)");
            });
        }
    }

}
