using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehmini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFieldsForResetPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PwdResetTokenCreationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PwdResetTokenCreationDate",
                table: "AspNetUsers");
        }
    }
}
