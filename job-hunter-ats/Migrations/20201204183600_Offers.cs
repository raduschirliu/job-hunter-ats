using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace job_hunter_ats.Migrations
{
    public partial class Offers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    ApplicationId = table.Column<long>(nullable: false),
                    OfferId = table.Column<long>(nullable: false),
                    AcceptanceEndDate = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => new { x.ApplicationId, x.OfferId });
                    table.ForeignKey(
                        name: "FK_Offers_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "ApplicationId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offers");
        }
    }
}
