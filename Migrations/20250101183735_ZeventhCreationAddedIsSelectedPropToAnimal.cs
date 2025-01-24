using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DierenManagement.Migrations
{
    /// <inheritdoc />
    public partial class ZeventhCreationAddedIsSelectedPropToAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2154bae0-da5e-4d1d-be15-1d4bdfeee075");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b92e1a03-377f-4815-b2bf-89c9c77db9af");

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "Animals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02eea71c-bbe3-45f0-b1da-68f81e984e0e", null, "Client", "Client" },
                    { "61903bd6-4c0d-46dc-b1c3-f8706e65bbe6", null, "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02eea71c-bbe3-45f0-b1da-68f81e984e0e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61903bd6-4c0d-46dc-b1c3-f8706e65bbe6");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "Animals");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2154bae0-da5e-4d1d-be15-1d4bdfeee075", null, "Admin", "Admin" },
                    { "b92e1a03-377f-4815-b2bf-89c9c77db9af", null, "Client", "Client" }
                });
        }
    }
}
