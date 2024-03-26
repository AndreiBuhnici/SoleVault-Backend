using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobyLabWebProgramming.Infrastructure.Migrations
{
    public partial class UserCartObjectUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Cart_CartId1",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_CartId1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CartId1",
                table: "User");

            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "User",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CartId",
                table: "User",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CartId1",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CartId1",
                table: "User",
                column: "CartId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Cart_CartId1",
                table: "User",
                column: "CartId1",
                principalTable: "Cart",
                principalColumn: "Id");
        }
    }
}
