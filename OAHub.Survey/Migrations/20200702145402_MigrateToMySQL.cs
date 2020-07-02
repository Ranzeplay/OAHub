using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Survey.Migrations
{
    public partial class MigrateToMySQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StandardAnswers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ForFormId = table.Column<string>(nullable: true),
                    Submitter = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    SubmitTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardAnswers", x => x.Id);
                });

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
                    IsPublished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Secret = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyOrganizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardAnswers");

            migrationBuilder.DropTable(
                name: "StandardForms");

            migrationBuilder.DropTable(
                name: "SurveyOrganizations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
