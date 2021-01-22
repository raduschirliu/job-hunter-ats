using Microsoft.EntityFrameworkCore.Migrations;

namespace job_hunter_ats.Migrations
{
    public partial class AwardValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "value",
                table: "Awards",
                newName: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Awards",
                newName: "value");
        }
    }
}
