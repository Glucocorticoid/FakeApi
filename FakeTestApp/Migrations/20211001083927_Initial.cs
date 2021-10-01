using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeTestApp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserData_UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserData_TimeFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserData_TimeTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestLocalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QueryId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
