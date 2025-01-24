using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DierenManagement.Migrations
{
    /// <inheritdoc />
    public partial class EighthCreationUpdatedBookingTableDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02eea71c-bbe3-45f0-b1da-68f81e984e0e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61903bd6-4c0d-46dc-b1c3-f8706e65bbe6");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "bookings",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "136d7063-9183-4adf-a001-d303d429f301", null, "Admin", "Admin" },
                    { "89cb15ab-ae48-479f-9375-e0e58bce201c", null, "Client", "Client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "136d7063-9183-4adf-a001-d303d429f301");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89cb15ab-ae48-479f-9375-e0e58bce201c");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02eea71c-bbe3-45f0-b1da-68f81e984e0e", null, "Client", "Client" },
                    { "61903bd6-4c0d-46dc-b1c3-f8706e65bbe6", null, "Admin", "Admin" }
                });
        }
    }
}
