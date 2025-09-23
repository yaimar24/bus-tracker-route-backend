using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bus_tracker_route.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusPositions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    SpeedKph = table.Column<double>(type: "float", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusPositions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusPositions_BusId",
                table: "BusPositions",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusPositions_Route",
                table: "BusPositions",
                column: "Route");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusPositions");
        }
    }
}
