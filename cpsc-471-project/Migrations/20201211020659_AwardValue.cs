using Microsoft.EntityFrameworkCore.Migrations;

namespace cpsc_471_project.Migrations
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
