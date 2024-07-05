using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qr_scanner_app_staj.Migrations
{
    /// <inheritdoc />
    public partial class userIdAddedToReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "Receipt",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "Receipt");
        }
    }
}
