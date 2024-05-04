using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class addStatusToTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "License",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "License",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Car",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "License");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "License");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Car");
        }
    }
}
