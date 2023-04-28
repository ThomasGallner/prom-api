﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace prom_api.Migrations
{
    [DbContext(typeof(PromContext))]
    [Migration("20230428082051_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Purchase", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("TotalPrice")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("Student", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("InvitationCode")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("ID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Teacher", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("InvitationCode")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<bool>("TeacherOf5thGrade")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("Teachers");
                });

            modelBuilder.Entity("Ticket", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PurchaseID")
                        .HasColumnType("int");

                    b.Property<int?>("StudentID")
                        .HasColumnType("int");

                    b.Property<int?>("TeacherID")
                        .HasColumnType("int");

                    b.Property<int>("TicketTypeID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("PurchaseID");

                    b.HasIndex("StudentID");

                    b.HasIndex("TeacherID");

                    b.HasIndex("TicketTypeID");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("TicketType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("TicketTypes");
                });

            modelBuilder.Entity("Ticket", b =>
                {
                    b.HasOne("Purchase", "Purchase")
                        .WithMany("Tickets")
                        .HasForeignKey("PurchaseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Student", "Student")
                        .WithMany("Tickets")
                        .HasForeignKey("StudentID");

                    b.HasOne("Teacher", "Teacher")
                        .WithMany("Tickets")
                        .HasForeignKey("TeacherID");

                    b.HasOne("TicketType", "TicketType")
                        .WithMany("Tickets")
                        .HasForeignKey("TicketTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Purchase");

                    b.Navigation("Student");

                    b.Navigation("Teacher");

                    b.Navigation("TicketType");
                });

            modelBuilder.Entity("Purchase", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Student", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Teacher", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("TicketType", b =>
                {
                    b.Navigation("Tickets");
                });
#pragma warning restore 612, 618
        }
    }
}
