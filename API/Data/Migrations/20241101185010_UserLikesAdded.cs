using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserLikesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_LikedUserId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LikedUserId",
                table: "Likes",
                newName: "TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_LikedUserId",
                table: "Likes",
                newName: "IX_Likes_TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_TargetUserId",
                table: "Likes",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_TargetUserId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "Likes",
                newName: "LikedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_TargetUserId",
                table: "Likes",
                newName: "IX_Likes_LikedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_LikedUserId",
                table: "Likes",
                column: "LikedUserId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
