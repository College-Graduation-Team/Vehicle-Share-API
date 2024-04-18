using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class AddCreatedOnToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_UserData_UserDataId",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_License_UserData_UserDataId",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Trip_TripId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_UserData_UserDataId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Car_CarId",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_UserData_UserDataId",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_AspNetUsers_UserId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_UserId",
                table: "UserData");

            migrationBuilder.RenameColumn(
                name: "Birthdata",
                table: "UserData",
                newName: "CreatedOn");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserData",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthdate",
                table: "UserData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Trip",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CarId",
                table: "Trip",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Trip",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Request",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TripId",
                table: "Request",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Request",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshToken",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "License",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "License",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Car",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Car",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserId",
                table: "UserData",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_UserData_UserDataId",
                table: "Car",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_License_UserData_UserDataId",
                table: "License",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Trip_TripId",
                table: "Request",
                column: "TripId",
                principalTable: "Trip",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_UserData_UserDataId",
                table: "Request",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Car_CarId",
                table: "Trip",
                column: "CarId",
                principalTable: "Car",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_UserData_UserDataId",
                table: "Trip",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_AspNetUsers_UserId",
                table: "UserData",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_UserData_UserDataId",
                table: "Car");

            migrationBuilder.DropForeignKey(
                name: "FK_License_UserData_UserDataId",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Trip_TripId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_UserData_UserDataId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_Car_CarId",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_UserData_UserDataId",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_UserData_AspNetUsers_UserId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_UserId",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "Birthdate",
                table: "UserData");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "License");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "UserData",
                newName: "Birthdata");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Trip",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarId",
                table: "Trip",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Request",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TripId",
                table: "Request",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "RefreshToken",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "License",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserDataId",
                table: "Car",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserId",
                table: "UserData",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Car_UserData_UserDataId",
                table: "Car",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_License_UserData_UserDataId",
                table: "License",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Trip_TripId",
                table: "Request",
                column: "TripId",
                principalTable: "Trip",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_UserData_UserDataId",
                table: "Request",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_Car_CarId",
                table: "Trip",
                column: "CarId",
                principalTable: "Car",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_UserData_UserDataId",
                table: "Trip",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_AspNetUsers_UserId",
                table: "UserData",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
