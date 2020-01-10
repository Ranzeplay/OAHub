using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Organization.Migrations
{
    public partial class InitOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OfficialWebsite = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    FounderId = table.Column<string>(nullable: true),
                    Members = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
