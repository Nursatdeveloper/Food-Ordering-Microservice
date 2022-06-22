using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Migrations
{
    public partial class DeliveryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptedByDelivery",
                table: "FoodOrders");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryCode",
                table: "FoodOrders",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryCode",
                table: "FoodOrders");

            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedByDelivery",
                table: "FoodOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
