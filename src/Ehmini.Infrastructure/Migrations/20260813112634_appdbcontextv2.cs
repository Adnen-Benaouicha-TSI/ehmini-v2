using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehmini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class appdbcontextv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneCode",
                table: "country");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                table: "country",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
