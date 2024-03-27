using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobyLabWebProgramming.Infrastructure.Migrations
{
    public partial class FeedbackForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackFormId",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FeedbackForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Feedback = table.Column<string>(type: "text", nullable: false),
                    OverallRating = table.Column<int>(type: "integer", nullable: false),
                    DeliveryRating = table.Column<int>(type: "integer", nullable: false),
                    FavoriteFeatures = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackForm", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_FeedbackFormId",
                table: "User",
                column: "FeedbackFormId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_FeedbackForm_FeedbackFormId",
                table: "User",
                column: "FeedbackFormId",
                principalTable: "FeedbackForm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_FeedbackForm_FeedbackFormId",
                table: "User");

            migrationBuilder.DropTable(
                name: "FeedbackForm");

            migrationBuilder.DropIndex(
                name: "IX_User_FeedbackFormId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "FeedbackFormId",
                table: "User");
        }
    }
}
