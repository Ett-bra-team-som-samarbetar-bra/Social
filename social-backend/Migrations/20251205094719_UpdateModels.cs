using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_Users_FollowersId",
                table: "UserUser");

            migrationBuilder.DropForeignKey(
                name: "FK_UserUser_Users_FollowingId",
                table: "UserUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser");

            migrationBuilder.RenameTable(
                name: "UserUser",
                newName: "UserFollowers");

            migrationBuilder.RenameIndex(
                name: "IX_UserUser_FollowingId",
                table: "UserFollowers",
                newName: "IX_UserFollowers_FollowingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers",
                columns: new[] { "FollowersId", "FollowingId" });

            migrationBuilder.CreateTable(
                name: "UserLikedPosts",
                columns: table => new
                {
                    LikedById = table.Column<int>(type: "INTEGER", nullable: false),
                    LikedPostsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedPosts", x => new { x.LikedById, x.LikedPostsId });
                    table.ForeignKey(
                        name: "FK_UserLikedPosts_Posts_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLikedPosts_Users_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLikedPosts_LikedPostsId",
                table: "UserLikedPosts",
                column: "LikedPostsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_Users_FollowersId",
                table: "UserFollowers",
                column: "FollowersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowers_Users_FollowingId",
                table: "UserFollowers",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_Users_FollowersId",
                table: "UserFollowers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowers_Users_FollowingId",
                table: "UserFollowers");

            migrationBuilder.DropTable(
                name: "UserLikedPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollowers",
                table: "UserFollowers");

            migrationBuilder.RenameTable(
                name: "UserFollowers",
                newName: "UserUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowers_FollowingId",
                table: "UserUser",
                newName: "IX_UserUser_FollowingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserUser",
                table: "UserUser",
                columns: new[] { "FollowersId", "FollowingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_Users_FollowersId",
                table: "UserUser",
                column: "FollowersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserUser_Users_FollowingId",
                table: "UserUser",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
