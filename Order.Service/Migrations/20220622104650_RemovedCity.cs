using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Service.Migrations
{
    public partial class RemovedCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "FoodOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "FoodOrders",
                type: "text",
                nullable: true);
        }
    }
}
