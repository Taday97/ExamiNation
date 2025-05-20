using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamiNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres");

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres");

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
