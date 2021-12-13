﻿// <auto-generated />
using System;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LibraryManagementSystem.Migrations
{
    [DbContext(typeof(LibraryDbContext))]
    [Migration("20211213180058_Added Library Changes-1.0")]
    partial class AddedLibraryChanges10
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LibraryMSystem.Data.Models.AvailabilityStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AvailabilityStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "The item is lost.",
                            Name = "LOST"
                        },
                        new
                        {
                            Id = 2,
                            Description = "The item is in good condition.",
                            Name = "GOOD_CONDITION"
                        },
                        new
                        {
                            Id = 3,
                            Description = "The item is in unknown whereabouts and condition.",
                            Name = "UNKNOWN_CONDITION"
                        },
                        new
                        {
                            Id = 4,
                            Description = "The item has been destroyed.",
                            Name = "DESTROYED"
                        });
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("AvailabilityStatusId")
                        .HasColumnType("int");

                    b.Property<byte[]>("BookImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("CopiesAvailable")
                        .HasColumnType("int");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("DeweyIndex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Edition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Language")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<int>("NoOfPages_LengthTime")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfCopies")
                        .HasColumnType("int");

                    b.Property<int>("PublicationYear")
                        .HasColumnType("int");

                    b.Property<string>("Publisher")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AvailabilityStatusId");

                    b.HasIndex("LocationId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.BranchHours", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("CloseTime")
                        .HasColumnType("int");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("int");

                    b.Property<int>("OpenTime")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.ToTable("BranchHours");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Checkout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CheckedOutSince")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckedOutUntil")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LibraryCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("LibraryCardId");

                    b.ToTable("Checkouts");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.CheckoutHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CheckedIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckedOut")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LibraryCardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("LibraryCardId");

                    b.ToTable("CheckoutHistories");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.LibraryBranch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("BranchImage")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfAssets")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfPatrons")
                        .HasColumnType("int");

                    b.Property<DateTime>("OpenDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TotalAssetValue")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("Id");

                    b.ToTable("LibraryBranches");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.LibraryCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("CurrentFees")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime>("Issued")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("LibraryCards");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Patron", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("HomeLibraryBranchId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("LibraryCardId")
                        .HasColumnType("int");

                    b.Property<decimal>("OverdueFees")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Telephone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("HomeLibraryBranchId");

                    b.HasIndex("LibraryCardId");

                    b.ToTable("Patrons");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("HoldPlaced")
                        .HasColumnType("datetime2");

                    b.Property<int?>("LibraryCardId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("LibraryCardId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Book", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.AvailabilityStatus", "AvailabilityStatus")
                        .WithMany()
                        .HasForeignKey("AvailabilityStatusId");

                    b.HasOne("LibraryMSystem.Data.Models.LibraryBranch", "Location")
                        .WithMany("LibraryBooks")
                        .HasForeignKey("LocationId");

                    b.Navigation("AvailabilityStatus");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.BranchHours", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.LibraryBranch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.Navigation("Branch");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Checkout", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId");

                    b.HasOne("LibraryMSystem.Data.Models.LibraryCard", "LibraryCard")
                        .WithMany("Checkouts")
                        .HasForeignKey("LibraryCardId");

                    b.Navigation("Book");

                    b.Navigation("LibraryCard");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.CheckoutHistory", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId");

                    b.HasOne("LibraryMSystem.Data.Models.LibraryCard", "LibraryCard")
                        .WithMany()
                        .HasForeignKey("LibraryCardId");

                    b.Navigation("Book");

                    b.Navigation("LibraryCard");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Patron", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.LibraryBranch", "HomeLibraryBranch")
                        .WithMany("Patrons")
                        .HasForeignKey("HomeLibraryBranchId");

                    b.HasOne("LibraryMSystem.Data.Models.LibraryCard", "LibraryCard")
                        .WithMany()
                        .HasForeignKey("LibraryCardId");

                    b.Navigation("HomeLibraryBranch");

                    b.Navigation("LibraryCard");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.Reservation", b =>
                {
                    b.HasOne("LibraryMSystem.Data.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookId");

                    b.HasOne("LibraryMSystem.Data.Models.LibraryCard", "LibraryCard")
                        .WithMany()
                        .HasForeignKey("LibraryCardId");

                    b.Navigation("Book");

                    b.Navigation("LibraryCard");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.LibraryBranch", b =>
                {
                    b.Navigation("LibraryBooks");

                    b.Navigation("Patrons");
                });

            modelBuilder.Entity("LibraryMSystem.Data.Models.LibraryCard", b =>
                {
                    b.Navigation("Checkouts");
                });
#pragma warning restore 612, 618
        }
    }
}
