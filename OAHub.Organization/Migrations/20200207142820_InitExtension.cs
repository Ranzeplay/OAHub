using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Organization.Migrations
{
    public partial class InitExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtensionsInstalled",
                table: "Organizations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Extensions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    WebSite = table.Column<string>(nullable: true),
                    OrganizationRootUri = table.Column<string>(nullable: true),
                    CreateDashboardUri = table.Column<string>(nullable: true),
                    AuthorId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extensions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Extensions");

            migrationBuilder.DropColumn(
                name: "ExtensionsInstalled",
                table: "Organizations");
        }
    }
}
