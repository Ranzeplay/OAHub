using Microsoft.EntityFrameworkCore.Migrations;

namespace OAHub.Workflow.Migrations
{
    public partial class OrganizationConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowOrganizations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Secret = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowOrganizations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowOrganizations");
        }
    }
}
