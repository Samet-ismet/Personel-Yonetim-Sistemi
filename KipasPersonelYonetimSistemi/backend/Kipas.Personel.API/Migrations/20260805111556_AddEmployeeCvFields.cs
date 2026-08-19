using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipas.Personel.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeCvFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CvContentType",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CvFileSize",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvOriginalFileName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvStoredFileName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CvUploadedAt",
                table: "Employees",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvContentType",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CvFileSize",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CvOriginalFileName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CvStoredFileName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CvUploadedAt",
                table: "Employees");
        }
    }
}
