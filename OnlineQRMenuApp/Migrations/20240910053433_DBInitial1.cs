using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineQRMenuApp.Migrations
{
    /// <inheritdoc />
    public partial class DBInitial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_CoffeeShops_CoffeeShopId",
                table: "MenuItems");

            migrationBuilder.AlterColumn<int>(
                name: "CoffeeShopId",
                table: "MenuItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CoffeeShopId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CoffeeShopId",
                table: "Categories",
                column: "CoffeeShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_CoffeeShops_CoffeeShopId",
                table: "Categories",
                column: "CoffeeShopId",
                principalTable: "CoffeeShops",
                principalColumn: "CoffeeShopId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_CoffeeShops_CoffeeShopId",
                table: "MenuItems",
                column: "CoffeeShopId",
                principalTable: "CoffeeShops",
                principalColumn: "CoffeeShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CoffeeShops_CoffeeShopId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_CoffeeShops_CoffeeShopId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CoffeeShopId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CoffeeShopId",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "CoffeeShopId",
                table: "MenuItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_CoffeeShops_CoffeeShopId",
                table: "MenuItems",
                column: "CoffeeShopId",
                principalTable: "CoffeeShops",
                principalColumn: "CoffeeShopId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
