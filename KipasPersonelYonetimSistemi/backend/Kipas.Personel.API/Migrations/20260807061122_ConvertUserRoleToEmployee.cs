using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipas.Personel.API.Migrations
{
    /// <inheritdoc />
    public partial class ConvertUserRoleToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Users] " +
                "SET [Role] = 'Employee' " +
                "WHERE [Role] = 'User';");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE [Users] " +
                "SET [Role] = 'User' " +
                "WHERE [Role] = 'Employee';");
        }
    }
}
