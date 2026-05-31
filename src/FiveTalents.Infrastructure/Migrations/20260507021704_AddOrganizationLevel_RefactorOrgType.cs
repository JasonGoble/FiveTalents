using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddOrganizationLevel_RefactorOrgType : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Type",
            table: "Organizations",
            newName: "Level");

        // Shift 0-based enum values to 1-based level numbers
        // (National=0→1, Diocese=1→2, Church=2→3)
        migrationBuilder.Sql("UPDATE \"Organizations\" SET \"Level\" = \"Level\" + 1");

        migrationBuilder.CreateTable(
            name: "OrganizationLevels",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Level = table.Column<int>(type: "integer", nullable: false),
                DisplayName = table.Column<string>(type: "text", nullable: false),
                PluralDisplayName = table.Column<string>(type: "text", nullable: false),
                IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_OrganizationLevels", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_OrganizationLevels_Level",
            table: "OrganizationLevels",
            column: "Level",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "OrganizationLevels");

        migrationBuilder.RenameColumn(
            name: "Level",
            table: "Organizations",
            newName: "Type");
    }
}
