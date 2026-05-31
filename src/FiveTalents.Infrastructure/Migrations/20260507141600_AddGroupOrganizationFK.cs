using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddGroupOrganizationFK : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Groups_OrganizationId",
            table: "Groups",
            column: "OrganizationId");

        migrationBuilder.AddForeignKey(
            name: "FK_Groups_Organizations_OrganizationId",
            table: "Groups",
            column: "OrganizationId",
            principalTable: "Organizations",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Groups_Organizations_OrganizationId",
            table: "Groups");

        migrationBuilder.DropIndex(
            name: "IX_Groups_OrganizationId",
            table: "Groups");
    }
}
