using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reparo.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAiSummaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiSummary",
                table: "FaultReports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiSummaryGeneratedAt",
                table: "FaultReports",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiSummary",
                table: "FaultReports");

            migrationBuilder.DropColumn(
                name: "AiSummaryGeneratedAt",
                table: "FaultReports");
        }
    }
}
