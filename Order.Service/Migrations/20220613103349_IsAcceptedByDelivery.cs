using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Migrations
{
    public partial class IsAcceptedByDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedByDelivery",
                table: "FoodOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptedByDelivery",
                table: "FoodOrders");
        }
    }
}
