using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dio_net_minimals_api.Migrations
{
    /// <inheritdoc />
    public partial class TestChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "admin1234");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Administrators",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "admin123");
        }
    }
}
