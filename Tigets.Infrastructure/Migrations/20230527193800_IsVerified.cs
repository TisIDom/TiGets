using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tigets.Infrastructure.Migrations
{
    public partial class IsVerified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isVerified",
                table: "AspNetUsers");
        }
    }
}
