using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Survey.Migrations
{
    public partial class initPublishedArg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "StandardForms",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "StandardForms");
        }
    }
}
