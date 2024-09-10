using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineQRMenuApp.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemCustomizations");

            migrationBuilder.RenameColumn(
                name: "SizeOption",
                table: "OrderItems",
                newName: "SizeOptions");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SizeOptions",
                table: "OrderItems",
                newName: "SizeOption");

            migrationBuilder.AlterColumn<int>(
                name: "Note",
                table: "OrderItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "OrderItemCustomizations",
                columns: table => new
                {
                    OrderItemCustomizationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemCustomizationId = table.Column<int>(type: "int", nullable: false),
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemCustomizations", x => x.OrderItemCustomizationId);
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_MenuItemCustomizations_MenuItemCustomizationId",
                        column: x => x.MenuItemCustomizationId,
                        principalTable: "MenuItemCustomizations",
                        principalColumn: "MenuItemCustomizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemCustomizations_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "OrderItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_MenuItemCustomizationId",
                table: "OrderItemCustomizations",
                column: "MenuItemCustomizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemCustomizations_OrderItemId",
                table: "OrderItemCustomizations",
                column: "OrderItemId");
        }
    }
}
