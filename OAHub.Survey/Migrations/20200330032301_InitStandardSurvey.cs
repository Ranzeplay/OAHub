using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Survey.Migrations
{
    public partial class InitStandardSurvey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StandardForms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    AuthorId = table.Column<string>(nullable: true),
                    AllowAnonymous = table.Column<bool>(nullable: false),
                    AllowMultipleSubmits = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Submitters = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardForms", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardForms");
        }
    }
}
