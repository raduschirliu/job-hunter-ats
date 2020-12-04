using Microsoft.EntityFrameworkCore.Migrations;

namespace cpsc_471_project.Migrations
{
    public partial class referrals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    ApplicationId = table.Column<long>(nullable: false),
                    ReferralId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    Position = table.Column<string>(maxLength: 255, nullable: true),
                    Company = table.Column<string>(maxLength: 255, nullable: true),
                    Phone = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => new { x.ApplicationId, x.ReferralId });
                    table.ForeignKey(
                        name: "FK_Referrals_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Referrals");
        }
    }
}
