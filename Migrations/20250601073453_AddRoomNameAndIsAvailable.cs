using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomNameAndIsAvailable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9151));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "IsAvailable", "Name" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9354), "Luxurious deluxe room with ocean view", true, "Deluxe Ocean View" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description", "IsAvailable", "Name" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9364), "Spacious executive suite with premium amenities", true, "Executive Suite" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description", "IsAvailable", "Name" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9367), "Comfortable standard room with essential amenities", true, "Standard Comfort" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description", "IsAvailable", "Name" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9370), "Premium room with stunning city views", true, "Premium City View" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description", "IsAvailable", "Name" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9372), "Perfect family room with multiple beds and entertainment", true, "Family Paradise" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 11, 34, 52, 491, DateTimeKind.Local).AddTicks(9324));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Rooms");

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(3801));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4288), null });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4299), null });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4302), null });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4304), null });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4306), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 11, 8, 49, 397, DateTimeKind.Local).AddTicks(4240));
        }
    }
}
