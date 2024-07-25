using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteAlert.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLogoUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Vendors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
