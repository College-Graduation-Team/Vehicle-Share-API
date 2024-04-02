using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class Addstatustorequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccept",
                table: "Request");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "Request",
                type: "tinyint",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccept",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
