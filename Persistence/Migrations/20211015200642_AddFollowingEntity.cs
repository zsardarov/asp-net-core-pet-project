﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddFollowingEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFollowings",
                columns: table => new
                {
                    ObserverId = table.Column<string>(type: "TEXT", nullable: false),
                    TargetId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowing", x => new { x.ObserverId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_UserFollowing_AspNetUsers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollowing_AspNetUsers_TargetId",
                        column: x => x.TargetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowing_TargetId",
                table: "UserFollowing",
                column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowing");
        }
    }
}
