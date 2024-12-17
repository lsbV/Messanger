﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241217101124_m1")]
    partial class m1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Core.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("Core.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AuthorizationVersion")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7"),
                            AuthorizationVersion = 1,
                            Email = "User1@email.com",
                            Name = "User1"
                        },
                        new
                        {
                            Id = new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3"),
                            AuthorizationVersion = 1,
                            Email = "User2@email.com",
                            Name = "User2"
                        },
                        new
                        {
                            Id = new Guid("5defeb16-a804-4f86-a5ee-1bfe93d37853"),
                            AuthorizationVersion = 1,
                            Email = "User3@email.com",
                            Name = "User3"
                        });
                });

            modelBuilder.Entity("Database.Configurations.GroupChatUser", b =>
                {
                    b.Property<Guid>("GroupChatId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Role")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("GroupChatId", "UserId", "Role");

                    b.HasIndex("UserId");

                    b.ToTable("GroupChatUser");

                    b.HasData(
                        new
                        {
                            GroupChatId = new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"),
                            UserId = new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7"),
                            Role = "Owner"
                        },
                        new
                        {
                            GroupChatId = new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"),
                            UserId = new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3"),
                            Role = "User"
                        },
                        new
                        {
                            GroupChatId = new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"),
                            UserId = new Guid("5defeb16-a804-4f86-a5ee-1bfe93d37853"),
                            Role = "User"
                        });
                });

            modelBuilder.Entity("Core.GroupChat", b =>
                {
                    b.HasBaseType("Core.Chat");

                    b.Property<string>("ChatName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("GroupChats");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a7373d57-3abb-4c53-ab39-23a3faa4a2da"),
                            ChatName = "Band of 3"
                        });
                });

            modelBuilder.Entity("Core.PrivateChat", b =>
                {
                    b.HasBaseType("Core.Chat");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId1")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId2")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("PrivateChats");

                    b.HasData(
                        new
                        {
                            Id = new Guid("dc306013-c6f4-4158-84cc-601bc348c13a"),
                            UserId1 = new Guid("00ad8832-1c8e-4c5e-b0f5-338b619d62f7"),
                            UserId2 = new Guid("8aebfc91-ebe4-4080-b9f9-3f1c8312deb3")
                        });
                });

            modelBuilder.Entity("Database.Configurations.GroupChatUser", b =>
                {
                    b.HasOne("Core.GroupChat", null)
                        .WithMany()
                        .HasForeignKey("GroupChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Core.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Core.PrivateChat", b =>
                {
                    b.HasOne("Core.User", null)
                        .WithMany("PrivateChats")
                        .HasForeignKey("UserId");

                    b.HasOne("Core.User", "User1")
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Core.User", "User2")
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("Core.User", b =>
                {
                    b.Navigation("PrivateChats");
                });
#pragma warning restore 612, 618
        }
    }
}
