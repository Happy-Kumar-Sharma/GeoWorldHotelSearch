using Microsoft.EntityFrameworkCore.Migrations;

namespace GeoWorldHotelSearch.Migrations
{
    public partial class AddIsFeaturedToHotel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Hotels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Hotels");
        }
    }
}
