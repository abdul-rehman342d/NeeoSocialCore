using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class NEW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friend",
                columns: table => new
                {
                    FriendID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID1 = table.Column<long>(nullable: false),
                    UserID2 = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => x.FriendID);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequest",
                columns: table => new
                {
                    FriendRequestID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    fromReq = table.Column<long>(nullable: false),
                    toReq = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequest", x => x.FriendRequestID);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<long>(nullable: false),
                    text = table.Column<string>(nullable: true),
                    imageURL = table.Column<string>(nullable: true),
                    video = table.Column<byte[]>(nullable: true),
                    postTime = table.Column<DateTime>(nullable: false),
                    updateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    password = table.Column<string>(nullable: true),
                    dateOfBirth = table.Column<DateTime>(nullable: false),
                    gender = table.Column<int>(nullable: false),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    isVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserMedia",
                columns: table => new
                {
                    UserMediaID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<long>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMedia", x => x.UserMediaID);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    commentText = table.Column<string>(nullable: true),
                    commentTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentID);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reaction",
                columns: table => new
                {
                    ReactionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    reactionType = table.Column<int>(nullable: false),
                    reactionTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reaction", x => x.ReactionID);
                    table.ForeignKey(
                        name: "FK_Reaction_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubComment",
                columns: table => new
                {
                    SubCommentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentID = table.Column<int>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    commentText = table.Column<string>(nullable: true),
                    commentTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubComment", x => x.SubCommentID);
                    table.ForeignKey(
                        name: "FK_SubComment_Comment_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comment",
                        principalColumn: "CommentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubReaction",
                columns: table => new
                {
                    SubReactionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentID = table.Column<int>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    reactionType = table.Column<int>(nullable: false),
                    reactionTime = table.Column<DateTime>(nullable: false),
                    SubCommentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubReaction", x => x.SubReactionID);
                    table.ForeignKey(
                        name: "FK_SubReaction_Comment_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comment",
                        principalColumn: "CommentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubReaction_SubComment_SubCommentID",
                        column: x => x.SubCommentID,
                        principalTable: "SubComment",
                        principalColumn: "SubCommentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostID",
                table: "Comment",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_PostID",
                table: "Reaction",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_SubComment_CommentID",
                table: "SubComment",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubReaction_CommentID",
                table: "SubReaction",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubReaction_SubCommentID",
                table: "SubReaction",
                column: "SubCommentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friend");

            migrationBuilder.DropTable(
                name: "FriendRequest");

            migrationBuilder.DropTable(
                name: "Reaction");

            migrationBuilder.DropTable(
                name: "SubReaction");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserMedia");

            migrationBuilder.DropTable(
                name: "SubComment");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Post");
        }
    }
}
