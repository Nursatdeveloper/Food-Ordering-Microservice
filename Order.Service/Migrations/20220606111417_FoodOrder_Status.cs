using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Migrations
{
    public partial class FoodOrder_Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FoodOrders",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "FoodOrders");
        }
    }
}
