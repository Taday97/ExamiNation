using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamiNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tests");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "TestResults",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScoreRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MinScore = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    Classification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreRanges_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreRanges_TestId",
                table: "ScoreRanges",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreRanges");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "TestResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Tests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Tests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
