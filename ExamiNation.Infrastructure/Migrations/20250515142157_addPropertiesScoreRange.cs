using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamiNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPropertiesScoreRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailedExplanation",
                table: "ScoreRanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Recommendations",
                table: "ScoreRanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "ScoreRanges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailedExplanation",
                table: "ScoreRanges");

            migrationBuilder.DropColumn(
                name: "Recommendations",
                table: "ScoreRanges");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "ScoreRanges");
        }
    }
}
