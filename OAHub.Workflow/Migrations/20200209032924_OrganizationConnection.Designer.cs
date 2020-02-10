﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OAHub.Workflow.Data;

namespace OAHub.Workflow.Migrations
{
    [DbContext(typeof(WorkflowDbContext))]
    [Migration("20200209032924_OrganizationConnection")]
    partial class OrganizationConnection
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("OAHub.Base.Models.WorkflowModels.WorkflowOrganization", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Secret")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("WorkflowOrganizations");
                });
#pragma warning restore 612, 618
        }
    }
}
