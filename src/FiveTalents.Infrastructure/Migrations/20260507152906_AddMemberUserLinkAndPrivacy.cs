using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddMemberUserLinkAndPrivacy : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "ShareAddressWithNetwork",
            table: "Members",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "ShareEmailWithNetwork",
            table: "Members",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "SharePhoneWithNetwork",
            table: "Members",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_MemberId",
            table: "AspNetUsers",
            column: "MemberId",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_AspNetUsers_Members_MemberId",
            table: "AspNetUsers",
            column: "MemberId",
            principalTable: "Members",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_AspNetUsers_Members_MemberId",
            table: "AspNetUsers");

        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_MemberId",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "ShareAddressWithNetwork",
            table: "Members");

        migrationBuilder.DropColumn(
            name: "ShareEmailWithNetwork",
            table: "Members");

        migrationBuilder.DropColumn(
            name: "SharePhoneWithNetwork",
            table: "Members");
    }
}
