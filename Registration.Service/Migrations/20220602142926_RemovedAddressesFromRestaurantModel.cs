using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registration.Service.Migrations
{
    public partial class RemovedAddressesFromRestaurantModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Restaurants_RestaurantId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_RestaurantId",
                table: "Addresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_RestaurantId",
                table: "Addresses",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Restaurants_RestaurantId",
                table: "Addresses",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
