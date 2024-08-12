using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineQRMenuApp.Migrations
{
    /// <inheritdoc />
    public partial class updateDB1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoffeeShops_Users_UserId",
                table: "CoffeeShops");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Categories");
        }
    }
}
