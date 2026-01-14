using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Journaway.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    BedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "travel_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ArrivalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpectedTravellerCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travel_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TravelGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Surname = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_travellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_travellers_travel_groups_TravelGroupId",
                        column: x => x.TravelGroupId,
                        principalTable: "travel_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rooms_HotelId_RoomCode",
                table: "rooms",
                columns: new[] { "HotelId", "RoomCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_travel_groups_HotelId_GroupId",
                table: "travel_groups",
                columns: new[] { "HotelId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_travellers_TravelGroupId_Surname_FirstName_DateOfBirth",
                table: "travellers",
                columns: new[] { "TravelGroupId", "Surname", "FirstName", "DateOfBirth" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "travellers");

            migrationBuilder.DropTable(
                name: "travel_groups");
        }
    }
}
