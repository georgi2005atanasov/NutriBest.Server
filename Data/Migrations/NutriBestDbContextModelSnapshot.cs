﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NutriBest.Server.Data;

#nullable disable

namespace NutriBest.Server.Data.Migrations
{
    [DbContext(typeof(NutriBestDbContext))]
    partial class NutriBestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PostalCode")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProfileUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StreetNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId")
                        .IsUnique();

                    b.HasIndex("ProfileUserId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("BrandLogoId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.HasIndex("BrandLogoId")
                        .IsUnique()
                        .HasFilter("[BrandLogoId] IS NOT NULL");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.BrandLogo", b =>
                {
                    b.Property<int>("BrandLogoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandLogoId"), 1L, 1);

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrandLogoId");

                    b.ToTable("BrandLogo");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.CartProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.ToTable("CartProducts");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.NutritionFacts", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<double?>("Carbohydrates")
                        .HasColumnType("float");

                    b.Property<double?>("EnergyValue")
                        .HasColumnType("float");

                    b.Property<double?>("Fats")
                        .HasColumnType("float");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double?>("Proteins")
                        .HasColumnType("float");

                    b.Property<double?>("Salt")
                        .HasColumnType("float");

                    b.Property<double?>("SaturatedFats")
                        .HasColumnType("float");

                    b.Property<double?>("Sugars")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.ToTable("NutritionFacts");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("ProfileId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CartId")
                        .IsUnique();

                    b.HasIndex("ProfileId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.OrderDetails", b =>
                {
                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<bool>("IsShipped")
                        .HasColumnType("bit");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("AddressId");

                    b.ToTable("OrdersDetails");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<int?>("BrandId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductImageId")
                        .HasColumnType("int");

                    b.Property<int?>("PromotionId")
                        .HasColumnType("int");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("BrandId");

                    b.HasIndex("ProductImageId")
                        .IsUnique();

                    b.HasIndex("PromotionId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductCategory", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("ProductId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ProductsCategories");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductDetails", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("HowToUse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ServingSize")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServingsPerContainer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.ToTable("ProductsDetails");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductImage", b =>
                {
                    b.Property<int>("ProductImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductImageId"), 1L, 1);

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("ProductImageId");

                    b.ToTable("ProductsImages");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Profile", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<int?>("CartId")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("UserId");

                    b.HasIndex("CartId")
                        .IsUnique()
                        .HasFilter("[CartId] IS NOT NULL");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Promotion", b =>
                {
                    b.Property<int>("PromotionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PromotionId"), 1L, 1);

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal?>("DiscountAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("DiscountPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<decimal?>("MinimumPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("PromotionId");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Address", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Profile", null)
                        .WithOne("Address")
                        .HasForeignKey("NutriBest.Server.Data.Models.Address", "ProfileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileUserId");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Brand", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.BrandLogo", "BrandLogo")
                        .WithOne("Brand")
                        .HasForeignKey("NutriBest.Server.Data.Models.Brand", "BrandLogoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("BrandLogo");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.CartProduct", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Cart", "Cart")
                        .WithMany("CartProducts")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.NutritionFacts", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Product", "Product")
                        .WithOne("NutritionFacts")
                        .HasForeignKey("NutriBest.Server.Data.Models.NutritionFacts", "ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Order", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Cart", "Cart")
                        .WithOne()
                        .HasForeignKey("NutriBest.Server.Data.Models.Order", "CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.Profile", "Profile")
                        .WithMany("Orders")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.OrderDetails", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.Order", "Order")
                        .WithOne("OrderDetails")
                        .HasForeignKey("NutriBest.Server.Data.Models.OrderDetails", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Address");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Product", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("NutriBest.Server.Data.Models.ProductImage", "ProductImage")
                        .WithOne("Product")
                        .HasForeignKey("NutriBest.Server.Data.Models.Product", "ProductImageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.Promotion", "Promotion")
                        .WithMany("Products")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Brand");

                    b.Navigation("ProductImage");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductCategory", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Category", "Category")
                        .WithMany("ProductsCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("NutriBest.Server.Data.Models.Product", "Product")
                        .WithMany("ProductsCategories")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductDetails", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Product", null)
                        .WithOne("ProductDetails")
                        .HasForeignKey("NutriBest.Server.Data.Models.ProductDetails", "ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Profile", b =>
                {
                    b.HasOne("NutriBest.Server.Data.Models.Cart", "Cart")
                        .WithOne("Profile")
                        .HasForeignKey("NutriBest.Server.Data.Models.Profile", "CartId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("NutriBest.Server.Data.Models.User", null)
                        .WithOne("Profile")
                        .HasForeignKey("NutriBest.Server.Data.Models.Profile", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.BrandLogo", b =>
                {
                    b.Navigation("Brand");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Cart", b =>
                {
                    b.Navigation("CartProducts");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Category", b =>
                {
                    b.Navigation("ProductsCategories");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Product", b =>
                {
                    b.Navigation("NutritionFacts")
                        .IsRequired();

                    b.Navigation("ProductDetails")
                        .IsRequired();

                    b.Navigation("ProductsCategories");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.ProductImage", b =>
                {
                    b.Navigation("Product")
                        .IsRequired();
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Profile", b =>
                {
                    b.Navigation("Address");

                    b.Navigation("Orders");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.Promotion", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("NutriBest.Server.Data.Models.User", b =>
                {
                    b.Navigation("Profile")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
