using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddGroupRelationships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GroupMembers_Members_MemberId",
            table: "GroupMembers");

        migrationBuilder.DropForeignKey(
            name: "FK_Groups_Members_CoLeaderId",
            table: "Groups");

        migrationBuilder.DropForeignKey(
            name: "FK_Groups_Members_LeaderId",
            table: "Groups");

        migrationBuilder.DropIndex(
            name: "IX_Groups_CoLeaderId",
            table: "Groups");

        migrationBuilder.DropIndex(
            name: "IX_Groups_LeaderId",
            table: "Groups");

        migrationBuilder.DropColumn(
            name: "CoLeaderId",
            table: "Groups");

        migrationBuilder.DropColumn(
            name: "LeaderId",
            table: "Groups");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_CoLeaderMemberId",
            table: "Groups",
            column: "CoLeaderMemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_LeaderMemberId",
            table: "Groups",
            column: "LeaderMemberId");

        migrationBuilder.AddForeignKey(
            name: "FK_GroupMembers_Members_MemberId",
            table: "GroupMembers",
            column: "MemberId",
            principalTable: "Members",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Groups_Members_CoLeaderMemberId",
            table: "Groups",
            column: "CoLeaderMemberId",
            principalTable: "Members",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_Groups_Members_LeaderMemberId",
            table: "Groups",
            column: "LeaderMemberId",
            principalTable: "Members",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_GroupMembers_Members_MemberId",
            table: "GroupMembers");

        migrationBuilder.DropForeignKey(
            name: "FK_Groups_Members_CoLeaderMemberId",
            table: "Groups");

        migrationBuilder.DropForeignKey(
            name: "FK_Groups_Members_LeaderMemberId",
            table: "Groups");

        migrationBuilder.DropIndex(
            name: "IX_Groups_CoLeaderMemberId",
            table: "Groups");

        migrationBuilder.DropIndex(
            name: "IX_Groups_LeaderMemberId",
            table: "Groups");

        migrationBuilder.AddColumn<int>(
            name: "CoLeaderId",
            table: "Groups",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "LeaderId",
            table: "Groups",
            type: "integer",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Groups_CoLeaderId",
            table: "Groups",
            column: "CoLeaderId");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_LeaderId",
            table: "Groups",
            column: "LeaderId");

        migrationBuilder.AddForeignKey(
            name: "FK_GroupMembers_Members_MemberId",
            table: "GroupMembers",
            column: "MemberId",
            principalTable: "Members",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Groups_Members_CoLeaderId",
            table: "Groups",
            column: "CoLeaderId",
            principalTable: "Members",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Groups_Members_LeaderId",
            table: "Groups",
            column: "LeaderId",
            principalTable: "Members",
            principalColumn: "Id");
    }
}
