﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestGeneratorAPI.DataAccess.Context;

#nullable disable

namespace TestGeneratorAPI.DataAccess.Migrations
{
    [DbContext(typeof(TestGeneratorDbContext))]
    partial class TestGeneratorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.PluginEntity", b =>
                {
                    b.Property<Guid>("PluginId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid");

                    b.HasKey("PluginId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Plugins");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.PluginReleaseEntity", b =>
                {
                    b.Property<Guid>("PluginReleaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PluginId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PublisherId")
                        .HasColumnType("uuid");

                    b.Property<string>("Runtime")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("PluginReleaseId");

                    b.HasIndex("PluginId");

                    b.HasIndex("PublisherId");

                    b.ToTable("PluginReleases");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.TokenEntity", b =>
                {
                    b.Property<Guid>("TokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string[]>("Permissions")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("TokenId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.PluginEntity", b =>
                {
                    b.HasOne("TestGeneratorAPI.DataAccess.Entities.UserEntity", "Owner")
                        .WithMany("Plugins")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.PluginReleaseEntity", b =>
                {
                    b.HasOne("TestGeneratorAPI.DataAccess.Entities.PluginEntity", "Plugin")
                        .WithMany("Releases")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestGeneratorAPI.DataAccess.Entities.UserEntity", "Publisher")
                        .WithMany("PluginReleases")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Plugin");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.PluginEntity", b =>
                {
                    b.Navigation("Releases");
                });

            modelBuilder.Entity("TestGeneratorAPI.DataAccess.Entities.UserEntity", b =>
                {
                    b.Navigation("PluginReleases");

                    b.Navigation("Plugins");
                });
#pragma warning restore 612, 618
        }
    }
}
