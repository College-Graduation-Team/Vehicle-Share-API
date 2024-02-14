using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Share.EF.Migrations
{
    public partial class smsotp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevokeOn",
                table: "RefreshToken",
                newName: "RevokedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevokedOn",
                table: "RefreshToken",
                newName: "RevokeOn");
        }
    }
}
