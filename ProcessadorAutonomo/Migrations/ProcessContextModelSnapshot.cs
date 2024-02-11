﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProcessadorAutonomo.Data;

#nullable disable

namespace AutonomousTaskProcessor.Migrations
{
    [DbContext(typeof(ProcessContext))]
    partial class ProcessContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ProcessadorAutonomo.Entities.Process", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Processes");
                });

            modelBuilder.Entity("ProcessadorAutonomo.Entities.SubProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProcessId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isConcluded")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProcessId");

                    b.ToTable("SubProcesses");
                });

            modelBuilder.Entity("ProcessadorAutonomo.Entities.SubProcess", b =>
                {
                    b.HasOne("ProcessadorAutonomo.Entities.Process", "Process")
                        .WithMany("SubProcesses")
                        .HasForeignKey("ProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Process");
                });

            modelBuilder.Entity("ProcessadorAutonomo.Entities.Process", b =>
                {
                    b.Navigation("SubProcesses");
                });
#pragma warning restore 612, 618
        }
    }
}
