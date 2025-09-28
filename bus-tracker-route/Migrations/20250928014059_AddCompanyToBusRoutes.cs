using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bus_tracker_route.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToBusRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "BusRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Buses",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_BusRoutes_CompanyId",
                table: "BusRoutes",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusRoutes_Companies_CompanyId",
                table: "BusRoutes",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Companies_CompanyId",
                table: "BusRoutes");

            migrationBuilder.DropIndex(
                name: "IX_BusRoutes_CompanyId",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "BusRoutes");

            migrationBuilder.AlterColumn<bool>(
                name: "Year",
                table: "Buses",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
