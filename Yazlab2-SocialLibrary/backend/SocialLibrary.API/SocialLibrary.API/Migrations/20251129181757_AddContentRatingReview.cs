using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialLibrary.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContentRatingReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId_ContentId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Contents");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Ratings",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "Creator",
                table: "Contents",
                newName: "Director");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Contents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Contents");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Ratings",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Director",
                table: "Contents",
                newName: "Creator");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Ratings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Contents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId_ContentId",
                table: "Reviews",
                columns: new[] { "UserId", "ContentId" },
                unique: true);
        }
    }
}
