using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class seeddata8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("15362906-0c31-421d-8795-daa70dd2c98a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f3004420-ba8d-479e-a638-cb8a27072e25"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("fed48284-0889-492e-a85a-44d36f024cd3"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("fa926ef5-07a3-4e33-9d6c-dfe1b4fefde8"));
            migrationBuilder.InsertData(
               table: "AspNetRoles",
               columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
               values: new object[,]
               {
                    { new Guid("3b28fddd-7b86-4c45-824b-6f12cc68c533"), null, "Writer", "WRİTER" },
                    { new Guid("6d86815e-72e4-4ebe-9c08-47cde814dbc4"), null, "User", "USER" },
                    { new Guid("a2085777-1d13-4656-bba1-eba5a231fc0a"), null, "Admin", "ADMİN" }
               });
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[] { 1, "Create-User", null, new Guid("a2085777-1d13-4656-bba1-eba5a231fc0a") });

           

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LastName", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("4a8790a1-8541-489a-8963-dc4f581036ff"), 0, "b19f8552-b854-4a51-b515-683c8ec6506e", "admin@admin.com", false, "KARAASAN", false, null, "Faruk", "ADMİN@ADMİN.COM", "ADMİN@ADMİN.COM", "AQAAAAIAAYagAAAAEEOO2VbEHVJ49ayDluqhiAWtsI9FcuAHqHt7/ORGGloeOnYKeTj5lABi8BNY3JaG9w==", null, false, "db4b522d-6096-4354-a5a3-3ab2113e76b4", false, "admin@admin.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("3b28fddd-7b86-4c45-824b-6f12cc68c533"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("6d86815e-72e4-4ebe-9c08-47cde814dbc4"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("a2085777-1d13-4656-bba1-eba5a231fc0a"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("4a8790a1-8541-489a-8963-dc4f581036ff"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("15362906-0c31-421d-8795-daa70dd2c98a"), null, "Admin", "ADMİN" },
                    { new Guid("f3004420-ba8d-479e-a638-cb8a27072e25"), null, "Writer", "WRİTER" },
                    { new Guid("fed48284-0889-492e-a85a-44d36f024cd3"), null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LastName", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("fa926ef5-07a3-4e33-9d6c-dfe1b4fefde8"), 0, "ea36b61b-feff-4233-8918-e9a30548bc37", "admin@admin.com", false, "KARAASAN", false, null, "Faruk", "ADMİN@ADMİN.COM", "ADMİN@ADMİN.COM", "AQAAAAIAAYagAAAAEFBF7GXZER/sAfjU+4+7oBMO7CG9nvcFrkOfoW+tATQr9uylXYyoJJN+cu5rWVAFgQ==", null, false, "4bf562a7-3252-40d2-bb81-32672c926fa4", false, "admin@admin.com" });
        }
    }
}
