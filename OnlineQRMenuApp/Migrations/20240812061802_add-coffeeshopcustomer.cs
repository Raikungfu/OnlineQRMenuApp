using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineQRMenuApp.Migrations
{
    /// <inheritdoc />
    public partial class addcoffeeshopcustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CoffeeShops",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoffeeShopCustomer",
                columns: table => new
                {
                    CoffeeShopCustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoffeeShopId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeShopCustomer", x => x.CoffeeShopCustomerId);
                    table.ForeignKey(
                        name: "FK_CoffeeShopCustomer_CoffeeShops_CoffeeShopId",
                        column: x => x.CoffeeShopId,
                        principalTable: "CoffeeShops",
                        principalColumn: "CoffeeShopId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoffeeShopCustomer_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeShops_UserId",
                table: "CoffeeShops",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeShopCustomer_CoffeeShopId",
                table: "CoffeeShopCustomer",
                column: "CoffeeShopId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeShopCustomer_UserId",
                table: "CoffeeShopCustomer",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoffeeShops_Users_UserId",
                table: "CoffeeShops",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoffeeShops_Users_UserId",
                table: "CoffeeShops");

            migrationBuilder.DropTable(
                name: "CoffeeShopCustomer");

            migrationBuilder.DropIndex(
                name: "IX_CoffeeShops_UserId",
                table: "CoffeeShops");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CoffeeShops");
        }
    }
}
