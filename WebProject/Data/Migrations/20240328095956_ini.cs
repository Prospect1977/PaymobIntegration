using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebProject.Data.Migrations
{
    public partial class ini : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Payment = table.Column<double>(type: "float", nullable: false),
                    NetPayment = table.Column<double>(type: "float", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    PaymentCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DataDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "iframe/kiosk")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_ProductId",
                table: "UserOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrders_UserId",
                table: "UserOrders",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserOrders");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
