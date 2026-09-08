using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ehmini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AppversionControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Platform = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LatestVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MinRequiredVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StoreUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppVersions",
                columns: new[] { "Id", "LatestVersion", "MinRequiredVersion", "Platform", "ReleaseNotes", "StoreUrl", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "1.0.0", "1.0.0", "android", "Initial release", "https://play.google.com/store/apps/details?id=com.ctama.ehmini", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "1.0.0", "1.0.0", "ios", "Initial release", "https://apps.apple.com/app/ehmini/id000000000", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersions");
        }
    }
}
