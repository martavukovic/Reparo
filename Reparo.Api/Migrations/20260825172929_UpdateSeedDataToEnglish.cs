using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reparo.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Employees_ReportedByEmployeeId",
                table: "FaultReports");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Locations_LocationId",
                table: "FaultReports");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkAssignments_Employees_TechnicianId",
                table: "WorkAssignments");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InterventionMaterials",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Low");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Medium");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "High");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Critical");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Submitted");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Reviewed");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Assigned");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "In Progress");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Resolved");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Closed");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Electrical");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Plumbing");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Heating");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Network");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Construction");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Other");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Planned");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "In Progress");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Completed");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Failed");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Administrative Building");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "School");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Healthcare Facility");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Warehouse");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Piece");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Meter");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Liter");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Package");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Employees_ReportedByEmployeeId",
                table: "FaultReports",
                column: "ReportedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Locations_LocationId",
                table: "FaultReports",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkAssignments_Employees_TechnicianId",
                table: "WorkAssignments",
                column: "TechnicianId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Employees_ReportedByEmployeeId",
                table: "FaultReports");

            migrationBuilder.DropForeignKey(
                name: "FK_FaultReports_Locations_LocationId",
                table: "FaultReports");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkAssignments_Employees_TechnicianId",
                table: "WorkAssignments");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InterventionMaterials",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Nizak");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Srednji");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Visok");

            migrationBuilder.UpdateData(
                table: "FaultPriorities",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Kritičan");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Zaprimljeno");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Pregledano");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Dodijeljeno");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "U radu");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Riješeno");

            migrationBuilder.UpdateData(
                table: "FaultStatuses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Zatvoreno");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Elektrika");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Voda");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Grijanje");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Mreža");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Građevinski radovi");

            migrationBuilder.UpdateData(
                table: "FaultTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Ostalo");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Planirana");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "U tijeku");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Završena");

            migrationBuilder.UpdateData(
                table: "InterventionStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Neuspješna");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Upravna zgrada");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Škola");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Zdravstvena ustanova");

            migrationBuilder.UpdateData(
                table: "LocationTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Skladište");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Komad");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Metar");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Litra");

            migrationBuilder.UpdateData(
                table: "MaterialUnits",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Paket");

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Employees_ReportedByEmployeeId",
                table: "FaultReports",
                column: "ReportedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FaultReports_Locations_LocationId",
                table: "FaultReports",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkAssignments_Employees_TechnicianId",
                table: "WorkAssignments",
                column: "TechnicianId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
