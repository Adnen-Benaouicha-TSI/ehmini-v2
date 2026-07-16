using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehmini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFieldsForConfirmationCompte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "AspNetUsers");
        }
    }
}
