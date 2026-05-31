using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddMultiContactInfo : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ContactTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Category = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ContactTypes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MemberAddresses",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                ContactTypeId = table.Column<int>(type: "integer", nullable: false),
                IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                AddressLine1 = table.Column<string>(type: "text", nullable: true),
                AddressLine2 = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                State = table.Column<string>(type: "text", nullable: true),
                PostalCode = table.Column<string>(type: "text", nullable: true),
                Country = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberAddresses", x => x.Id);
                table.ForeignKey(
                    name: "FK_MemberAddresses_ContactTypes_ContactTypeId",
                    column: x => x.ContactTypeId,
                    principalTable: "ContactTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MemberAddresses_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MemberEmails",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                ContactTypeId = table.Column<int>(type: "integer", nullable: false),
                IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberEmails", x => x.Id);
                table.ForeignKey(
                    name: "FK_MemberEmails_ContactTypes_ContactTypeId",
                    column: x => x.ContactTypeId,
                    principalTable: "ContactTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MemberEmails_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MemberPhones",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                ContactTypeId = table.Column<int>(type: "integer", nullable: false),
                IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                PhoneNumber = table.Column<string>(type: "text", nullable: false),
                IsMobile = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberPhones", x => x.Id);
                table.ForeignKey(
                    name: "FK_MemberPhones_ContactTypes_ContactTypeId",
                    column: x => x.ContactTypeId,
                    principalTable: "ContactTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MemberPhones_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "ContactTypes",
            columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "IsActive", "IsDeleted", "Name", "SortOrder", "UpdatedAt", "UpdatedBy" },
            values: new object[,]
            {
                { 1, 0, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Home", 10, null, null },
                { 2, 0, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Work", 20, null, null },
                { 3, 0, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Mailing", 30, null, null },
                { 4, 0, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Other", 40, null, null },
                { 5, 1, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Personal", 10, null, null },
                { 6, 1, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Work", 20, null, null },
                { 7, 1, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Other", 30, null, null },
                { 8, 2, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Home", 10, null, null },
                { 9, 2, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Work", 20, null, null },
                { 10, 2, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Mobile", 30, null, null },
                { 11, 2, new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, true, false, "Other", 40, null, null }
            });

        migrationBuilder.CreateIndex(
            name: "IX_MemberAddresses_ContactTypeId",
            table: "MemberAddresses",
            column: "ContactTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberAddresses_MemberId",
            table: "MemberAddresses",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberEmails_ContactTypeId",
            table: "MemberEmails",
            column: "ContactTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberEmails_MemberId",
            table: "MemberEmails",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberPhones_ContactTypeId",
            table: "MemberPhones",
            column: "ContactTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberPhones_MemberId",
            table: "MemberPhones",
            column: "MemberId");

        // Migrate existing single-value contact data before dropping old columns
        migrationBuilder.Sql(@"
                INSERT INTO ""MemberEmails"" (""MemberId"", ""ContactTypeId"", ""IsPrimary"", ""Email"", ""CreatedAt"", ""IsDeleted"")
                SELECT ""Id"", 5, true, ""Email"", NOW() AT TIME ZONE 'UTC', false
                FROM ""Members""
                WHERE ""Email"" IS NOT NULL AND ""Email"" <> '';

                INSERT INTO ""MemberPhones"" (""MemberId"", ""ContactTypeId"", ""IsPrimary"", ""PhoneNumber"", ""IsMobile"", ""CreatedAt"", ""IsDeleted"")
                SELECT ""Id"", 8, true, ""PhoneNumber"", false, NOW() AT TIME ZONE 'UTC', false
                FROM ""Members""
                WHERE ""PhoneNumber"" IS NOT NULL AND ""PhoneNumber"" <> '';

                INSERT INTO ""MemberPhones"" (""MemberId"", ""ContactTypeId"", ""IsPrimary"", ""PhoneNumber"", ""IsMobile"", ""CreatedAt"", ""IsDeleted"")
                SELECT ""Id"", 8, false, ""AlternatePhone"", false, NOW() AT TIME ZONE 'UTC', false
                FROM ""Members""
                WHERE ""AlternatePhone"" IS NOT NULL AND ""AlternatePhone"" <> '';

                INSERT INTO ""MemberAddresses"" (""MemberId"", ""ContactTypeId"", ""IsPrimary"", ""AddressLine1"", ""City"", ""State"", ""PostalCode"", ""Country"", ""CreatedAt"", ""IsDeleted"")
                SELECT ""Id"", 1, true, ""Address"", ""City"", ""State"", ""PostalCode"", ""Country"", NOW() AT TIME ZONE 'UTC', false
                FROM ""Members""
                WHERE ""Address"" IS NOT NULL AND ""Address"" <> '';
            ");

        migrationBuilder.DropColumn(name: "Address", table: "Members");
        migrationBuilder.DropColumn(name: "AlternatePhone", table: "Members");
        migrationBuilder.DropColumn(name: "City", table: "Members");
        migrationBuilder.DropColumn(name: "Country", table: "Members");
        migrationBuilder.DropColumn(name: "Email", table: "Members");
        migrationBuilder.DropColumn(name: "PhoneNumber", table: "Members");
        migrationBuilder.DropColumn(name: "PostalCode", table: "Members");
        migrationBuilder.DropColumn(name: "State", table: "Members");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MemberAddresses");

        migrationBuilder.DropTable(
            name: "MemberEmails");

        migrationBuilder.DropTable(
            name: "MemberPhones");

        migrationBuilder.DropTable(
            name: "ContactTypes");

        migrationBuilder.AddColumn<string>(
            name: "Address",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AlternatePhone",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "City",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Country",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Email",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PostalCode",
            table: "Members",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "State",
            table: "Members",
            type: "text",
            nullable: true);
    }
}
