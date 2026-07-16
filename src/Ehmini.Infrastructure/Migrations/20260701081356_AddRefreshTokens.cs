using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ehmini.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Locality_LocalityId",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Profession_ProfessionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Locality_Zone_ZoneId",
                table: "Locality");

            migrationBuilder.DropForeignKey(
                name: "FK_Region_Country_CountryId",
                table: "Region");

            migrationBuilder.DropForeignKey(
                name: "FK_Zone_Region_RegionId",
                table: "Zone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Zone",
                table: "Zone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Region",
                table: "Region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locality",
                table: "Locality");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Country",
                table: "Country");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Profession",
                table: "Profession");

            migrationBuilder.RenameTable(
                name: "Zone",
                newName: "zone");

            migrationBuilder.RenameTable(
                name: "Region",
                newName: "region");

            migrationBuilder.RenameTable(
                name: "Locality",
                newName: "locality");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "country");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "address");

            migrationBuilder.RenameTable(
                name: "Profession",
                newName: "proffession");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "zone",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "RegionId",
                table: "zone",
                newName: "region_id");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "zone",
                newName: "date_created");

            migrationBuilder.RenameIndex(
                name: "IX_Zone_RegionId",
                table: "zone",
                newName: "IX_zone_region_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "region",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "region",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "region",
                newName: "country_id");

            migrationBuilder.RenameIndex(
                name: "IX_Region_CountryId",
                table: "region",
                newName: "IX_region_country_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "locality",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "ZoneId",
                table: "locality",
                newName: "zone_id");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "locality",
                newName: "zip_code");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "locality",
                newName: "date_created");

            migrationBuilder.RenameIndex(
                name: "IX_Locality_ZoneId",
                table: "locality",
                newName: "IX_locality_zone_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "country",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "IsoCode",
                table: "country",
                newName: "iso_code");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "country",
                newName: "date_created");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "address",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "LocalityId",
                table: "address",
                newName: "locality_id");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "address",
                newName: "date_created");

            migrationBuilder.RenameIndex(
                name: "IX_Address_LocalityId",
                table: "address",
                newName: "IX_address_locality_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "proffession",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "proffession",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "proffession",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "RiskLevel",
                table: "proffession",
                newName: "risk_level");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "zone",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "region",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "locality",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "country",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "iso_code",
                table: "country",
                type: "char(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "address",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "proffession",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "proffession",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "proffession",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "risk_level",
                table: "proffession",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_delegation",
                table: "zone",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gouvernerat",
                table: "region",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_localite",
                table: "locality",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pays",
                table: "country",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_address_id",
                table: "address",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_proffession",
                table: "proffession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_address_locality_locality_id",
                table: "address",
                column: "locality_id",
                principalTable: "locality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_address_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_proffession_ProfessionId",
                table: "AspNetUsers",
                column: "ProfessionId",
                principalTable: "proffession",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_locality_zone_zone_id",
                table: "locality",
                column: "zone_id",
                principalTable: "zone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_region_country_country_id",
                table: "region",
                column: "country_id",
                principalTable: "country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_zone_region_region_id",
                table: "zone",
                column: "region_id",
                principalTable: "region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_address_locality_locality_id",
                table: "address");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_address_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_proffession_ProfessionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_locality_zone_zone_id",
                table: "locality");

            migrationBuilder.DropForeignKey(
                name: "FK_region_country_country_id",
                table: "region");

            migrationBuilder.DropForeignKey(
                name: "FK_zone_region_region_id",
                table: "zone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_delegation",
                table: "zone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gouvernerat",
                table: "region");

            migrationBuilder.DropPrimaryKey(
                name: "PK_localite",
                table: "locality");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pays",
                table: "country");

            migrationBuilder.DropPrimaryKey(
                name: "PK_address_id",
                table: "address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_proffession",
                table: "proffession");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "zone",
                newName: "Zone");

            migrationBuilder.RenameTable(
                name: "region",
                newName: "Region");

            migrationBuilder.RenameTable(
                name: "locality",
                newName: "Locality");

            migrationBuilder.RenameTable(
                name: "country",
                newName: "Country");

            migrationBuilder.RenameTable(
                name: "address",
                newName: "Address");

            migrationBuilder.RenameTable(
                name: "proffession",
                newName: "Profession");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Zone",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "region_id",
                table: "Zone",
                newName: "RegionId");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "Zone",
                newName: "DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_zone_region_id",
                table: "Zone",
                newName: "IX_Zone_RegionId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Region",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "Region",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "country_id",
                table: "Region",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_region_country_id",
                table: "Region",
                newName: "IX_Region_CountryId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Locality",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "zone_id",
                table: "Locality",
                newName: "ZoneId");

            migrationBuilder.RenameColumn(
                name: "zip_code",
                table: "Locality",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "Locality",
                newName: "DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_locality_zone_id",
                table: "Locality",
                newName: "IX_Locality_ZoneId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Country",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "iso_code",
                table: "Country",
                newName: "IsoCode");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "Country",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Address",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "locality_id",
                table: "Address",
                newName: "LocalityId");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "Address",
                newName: "DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_address_locality_id",
                table: "Address",
                newName: "IX_Address_LocalityId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Profession",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Profession",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "Profession",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "risk_level",
                table: "Profession",
                newName: "RiskLevel");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Zone",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Region",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Locality",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IsoCode",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Profession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldUnicode: false,
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Profession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Profession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RiskLevel",
                table: "Profession",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Zone",
                table: "Zone",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Region",
                table: "Region",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locality",
                table: "Locality",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Country",
                table: "Country",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profession",
                table: "Profession",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Locality_LocalityId",
                table: "Address",
                column: "LocalityId",
                principalTable: "Locality",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Profession_ProfessionId",
                table: "AspNetUsers",
                column: "ProfessionId",
                principalTable: "Profession",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Locality_Zone_ZoneId",
                table: "Locality",
                column: "ZoneId",
                principalTable: "Zone",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Region_Country_CountryId",
                table: "Region",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Zone_Region_RegionId",
                table: "Zone",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
