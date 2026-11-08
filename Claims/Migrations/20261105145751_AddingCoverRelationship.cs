using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Claims.Migrations
{
    /// <inheritdoc />
    public partial class AddingCoverRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Claims",
                newName: "ClaimId");

            migrationBuilder.AlterColumn<string>(
                name: "CoverId",
                table: "Claims",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Covers",
                columns: table => new
                {
                    CoverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Premium = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Covers", x => x.CoverId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_CoverId",
                table: "Claims",
                column: "CoverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Covers_CoverId",
                table: "Claims",
                column: "CoverId",
                principalTable: "Covers",
                principalColumn: "CoverId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Covers_CoverId",
                table: "Claims");

            migrationBuilder.DropTable(
                name: "Covers");

            migrationBuilder.DropIndex(
                name: "IX_Claims_CoverId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "ClaimId",
                table: "Claims",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "CoverId",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
