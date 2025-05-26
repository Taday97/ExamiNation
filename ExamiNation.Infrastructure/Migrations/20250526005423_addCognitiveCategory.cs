using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamiNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCognitiveCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CognitiveCategoryId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CognitiveCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TestTypeId = table.Column<int>(type: "int", nullable: false),
                    TestType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CognitiveCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CognitiveCategoryId",
                table: "Questions",
                column: "CognitiveCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_CognitiveCategory_CognitiveCategoryId",
                table: "Questions",
                column: "CognitiveCategoryId",
                principalTable: "CognitiveCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_CognitiveCategory_CognitiveCategoryId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "CognitiveCategory");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CognitiveCategoryId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CognitiveCategoryId",
                table: "Questions");
        }
    }
}
