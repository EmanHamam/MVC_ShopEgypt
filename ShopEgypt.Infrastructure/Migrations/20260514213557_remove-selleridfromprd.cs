using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopEgypt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeselleridfromprd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Seller",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_SellerId",
                table: "Product",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_SellerId",
                table: "Product");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Seller",
                table: "Product",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
