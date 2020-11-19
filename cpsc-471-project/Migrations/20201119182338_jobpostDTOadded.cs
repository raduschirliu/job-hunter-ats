using Microsoft.EntityFrameworkCore.Migrations;

namespace cpsc_471_project.Migrations
{
    public partial class jobpostDTOadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobPost_CompanyId",
                table: "JobPost",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobPost_RecruiterId",
                table: "JobPost",
                column: "RecruiterId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_JobId",
                table: "Application",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Application_JobPost_JobId",
                table: "Application",
                column: "JobId",
                principalTable: "JobPost",
                principalColumn: "JobPostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPost_Company_CompanyId",
                table: "JobPost",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobPost_Users_RecruiterId",
                table: "JobPost",
                column: "RecruiterId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Application_JobPost_JobId",
                table: "Application");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPost_Company_CompanyId",
                table: "JobPost");

            migrationBuilder.DropForeignKey(
                name: "FK_JobPost_Users_RecruiterId",
                table: "JobPost");

            migrationBuilder.DropIndex(
                name: "IX_JobPost_CompanyId",
                table: "JobPost");

            migrationBuilder.DropIndex(
                name: "IX_JobPost_RecruiterId",
                table: "JobPost");

            migrationBuilder.DropIndex(
                name: "IX_Application_JobId",
                table: "Application");
        }
    }
}
