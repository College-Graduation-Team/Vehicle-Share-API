using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class datarelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    UserDataID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationailID = table.Column<int>(type: "int", nullable: false),
                    BirthData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image_Front = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Image_back = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    nationalCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sets = table.Column<int>(type: "int", nullable: false),
                    Image_Front = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Image_back = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
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
                name: "LicenseUser",
                columns: table => new
                {
                    LicenseID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndData = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image_Front = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Image_back = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseUser", x => x.LicenseID);
                    table.ForeignKey(
                        name: "FK_LicenseUser_UserData_User_DataId",
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
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Request_UserData_User_DataId",
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
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false),
                    AllSets = table.Column<int>(type: "int", nullable: false),
                    AvilableSets = table.Column<int>(type: "int", nullable: false),
                    IsFinish = table.Column<bool>(type: "bit", nullable: false),
                    User_DataId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Car_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Request_Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.TripID);
                    table.ForeignKey(
                        name: "FK_Trip_Car_Car_Id",
                        column: x => x.Car_Id,
                        principalTable: "Car",
                        principalColumn: "CarID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trip_Request_Request_Id",
                        column: x => x.Request_Id,
                        principalTable: "Request",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Trip_UserData_User_DataId",
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
                name: "IX_LicenseUser_User_DataId",
                table: "LicenseUser",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_User_DataId",
                table: "Request",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_Car_Id",
                table: "Trip",
                column: "Car_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_Request_Id",
                table: "Trip",
                column: "Request_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_User_DataId",
                table: "Trip",
                column: "User_DataId");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_User_Id",
                table: "UserData",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseUser");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "Car");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "UserData");
        }
    }
}
