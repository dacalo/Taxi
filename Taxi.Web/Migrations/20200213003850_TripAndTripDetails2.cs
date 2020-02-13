using Microsoft.EntityFrameworkCore.Migrations;

namespace Taxi.Web.Migrations
{
    public partial class TripAndTripDetails2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tripDetails_Trips_TripId",
                table: "tripDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tripDetails",
                table: "tripDetails");

            migrationBuilder.RenameTable(
                name: "tripDetails",
                newName: "TripDetails");

            migrationBuilder.RenameIndex(
                name: "IX_tripDetails_TripId",
                table: "TripDetails",
                newName: "IX_TripDetails_TripId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripDetails",
                table: "TripDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TripDetails_Trips_TripId",
                table: "TripDetails",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDetails_Trips_TripId",
                table: "TripDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripDetails",
                table: "TripDetails");

            migrationBuilder.RenameTable(
                name: "TripDetails",
                newName: "tripDetails");

            migrationBuilder.RenameIndex(
                name: "IX_TripDetails_TripId",
                table: "tripDetails",
                newName: "IX_tripDetails_TripId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tripDetails",
                table: "tripDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tripDetails_Trips_TripId",
                table: "tripDetails",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
