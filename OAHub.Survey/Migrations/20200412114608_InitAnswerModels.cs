using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Survey.Migrations
{
    public partial class InitAnswerModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Submitters",
                table: "StandardForms");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandardAnswers");

            migrationBuilder.AddColumn<string>(
                name: "Submitters",
                table: "StandardForms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
