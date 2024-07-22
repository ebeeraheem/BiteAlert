using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteAlert.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusinessInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessEmail",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPhoneNumber",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessTagline",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessEmail",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BusinessPhoneNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BusinessTagline",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Vendors");
        }
    }
}
