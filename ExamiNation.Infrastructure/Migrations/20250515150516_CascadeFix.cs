using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamiNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_Options_OptionId",
                table: "Answeres");

            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_Options_OptionId",
                table: "Answeres",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId",
                table: "TestResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_Options_OptionId",
                table: "Answeres");

            migrationBuilder.DropForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_Options_OptionId",
                table: "Answeres",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Answeres_TestResults_TestResultId",
                table: "Answeres",
                column: "TestResultId",
                principalTable: "TestResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_UserId",
                table: "TestResults",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
