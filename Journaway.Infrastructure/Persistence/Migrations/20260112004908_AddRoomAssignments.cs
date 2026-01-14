using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Journaway.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    TravelGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    TravellerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_room_assignments_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_room_assignments_travel_groups_TravelGroupId",
                        column: x => x.TravelGroupId,
                        principalTable: "travel_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_room_assignments_travellers_TravellerId",
                        column: x => x.TravellerId,
                        principalTable: "travellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_HotelId_Date",
                table: "room_assignments",
                columns: new[] { "HotelId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_HotelId_Date_TravellerId",
                table: "room_assignments",
                columns: new[] { "HotelId", "Date", "TravellerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_HotelId_RoomId_Date",
                table: "room_assignments",
                columns: new[] { "HotelId", "RoomId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_HotelId_TravelGroupId_Date",
                table: "room_assignments",
                columns: new[] { "HotelId", "TravelGroupId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_RoomId",
                table: "room_assignments",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_TravelGroupId",
                table: "room_assignments",
                column: "TravelGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_room_assignments_TravellerId",
                table: "room_assignments",
                column: "TravellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_assignments");
        }
    }
}
