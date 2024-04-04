using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    UserDataID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationailID = table.Column<long>(type: "bigint", nullable: false),
                    BirthData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalcardImgFront = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalcardImgBack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    typeOfUser = table.Column<bool>(type: "bit", nullable: false),
                    User_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.UserDataID);
                    table.ForeignKey(
                        name: "FK_UserData_AspNetUsers_User_Id",
                        column: x => x.User_Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Car",
                columns: table => new
                {
                    CarID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Model = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CarPlate = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SetsOfCar = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    CarImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicCarImgFront = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicCarImgBack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDataOfCarLic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Car", x => x.CarID);
                    table.ForeignKey(
                        name: "FK_Car_UserData_User_DataId",
                        column: x => x.User_DataId,
                        principalTable: "UserData",
                        principalColumn: "UserDataID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    LicID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicUserImgFront = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicUserImgBack = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDataOfUserLic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.LicID);
                    table.ForeignKey(
                        name: "FK_License_UserData_User_DataId",
                        column: x => x.User_DataId,
                        principalTable: "UserData",
                        principalColumn: "UserDataID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    TripID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    From = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    To = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Recommendprice = table.Column<float>(type: "real", nullable: false),
                    AvilableSets = table.Column<int>(type: "int", nullable: true),
                    NumOfSetWant = table.Column<int>(type: "int", nullable: true),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Car_Id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.TripID);
                    table.ForeignKey(
                        name: "FK_Trip_Car_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Car",
                        principalColumn: "CarID");
                    table.ForeignKey(
                        name: "FK_Trip_UserData_User_DataId",
                        column: x => x.User_DataId,
                        principalTable: "UserData",
                        principalColumn: "UserDataID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAccept = table.Column<bool>(type: "bit", nullable: false),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Trip_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Request_Trip_Trip_Id",
                        column: x => x.Trip_Id,
                        principalTable: "Trip",
                        principalColumn: "TripID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_UserData_User_DataId",
                        column: x => x.User_DataId,
                        principalTable: "UserData",
                        principalColumn: "UserDataID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Car_User_DataId",
                table: "Car",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_License_User_DataId",
                table: "License",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_Trip_Id",
                table: "Request",
                column: "Trip_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Request_User_DataId",
                table: "Request",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_Car_Id",
                table: "Trip",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_User_DataId",
                table: "Trip",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_User_Id",
                table: "UserData",
                column: "User_Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
