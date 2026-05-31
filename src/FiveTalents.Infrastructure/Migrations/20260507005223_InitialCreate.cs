using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FiveTalents.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<string>(type: "text", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                PrimaryOrganizationId = table.Column<int>(type: "integer", nullable: true),
                MemberId = table.Column<int>(type: "integer", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AttendanceSessions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                SessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                ServiceNumber = table.Column<int>(type: "integer", nullable: true),
                Notes = table.Column<string>(type: "text", nullable: true),
                IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AttendanceSessions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "CommunicationTemplates",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                Type = table.Column<int>(type: "integer", nullable: false),
                Subject = table.Column<string>(type: "text", nullable: false),
                Body = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_CommunicationTemplates", x => x.Id));

        migrationBuilder.CreateTable(
            name: "DonationBatches",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                BatchDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_DonationBatches", x => x.Id));

        migrationBuilder.CreateTable(
            name: "DonationFunds",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                GlAccountCode = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_DonationFunds", x => x.Id));

        migrationBuilder.CreateTable(
            name: "GroupTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                Color = table.Column<string>(type: "text", nullable: true),
                IconName = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_GroupTypes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MemberFamilies",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FamilyName = table.Column<string>(type: "text", nullable: false),
                Address = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                State = table.Column<string>(type: "text", nullable: true),
                PostalCode = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_MemberFamilies", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Organizations",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                Type = table.Column<int>(type: "integer", nullable: false),
                ParentOrganizationId = table.Column<int>(type: "integer", nullable: true),
                LogoUrl = table.Column<string>(type: "text", nullable: true),
                Website = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                Email = table.Column<string>(type: "text", nullable: true),
                Address = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                State = table.Column<string>(type: "text", nullable: true),
                PostalCode = table.Column<string>(type: "text", nullable: true),
                Country = table.Column<string>(type: "text", nullable: true),
                TimeZone = table.Column<string>(type: "text", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Organizations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Organizations_Organizations_ParentOrganizationId",
                    column: x => x.ParentOrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "SermonSeries",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ImageUrl = table.Column<string>(type: "text", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_SermonSeries", x => x.Id));

        migrationBuilder.CreateTable(
            name: "VolunteerOpportunities",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                Department = table.Column<string>(type: "text", nullable: true),
                MinVolunteers = table.Column<int>(type: "integer", nullable: true),
                MaxVolunteers = table.Column<int>(type: "integer", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_VolunteerOpportunities", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                ProviderKey = table.Column<string>(type: "text", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<string>(type: "text", nullable: false),
                RoleId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<string>(type: "text", nullable: false),
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Members",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                FirstName = table.Column<string>(type: "text", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                MiddleName = table.Column<string>(type: "text", nullable: true),
                Email = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                AlternatePhone = table.Column<string>(type: "text", nullable: true),
                DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Gender = table.Column<int>(type: "integer", nullable: true),
                MaritalStatus = table.Column<int>(type: "integer", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                JoinDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                BaptismDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Address = table.Column<string>(type: "text", nullable: true),
                City = table.Column<string>(type: "text", nullable: true),
                State = table.Column<string>(type: "text", nullable: true),
                PostalCode = table.Column<string>(type: "text", nullable: true),
                Country = table.Column<string>(type: "text", nullable: true),
                ProfilePhotoUrl = table.Column<string>(type: "text", nullable: true),
                Occupation = table.Column<string>(type: "text", nullable: true),
                Employer = table.Column<string>(type: "text", nullable: true),
                EmergencyContactName = table.Column<string>(type: "text", nullable: true),
                EmergencyContactPhone = table.Column<string>(type: "text", nullable: true),
                Notes = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<string>(type: "text", nullable: true),
                FamilyId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Members", x => x.Id);
                table.ForeignKey(
                    name: "FK_Members_MemberFamilies_FamilyId",
                    column: x => x.FamilyId,
                    principalTable: "MemberFamilies",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "OrganizationSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                OrganizationId = table.Column<int>(type: "integer", nullable: false),
                PrimaryColor = table.Column<string>(type: "text", nullable: true),
                SecondaryColor = table.Column<string>(type: "text", nullable: true),
                Currency = table.Column<string>(type: "text", nullable: true),
                FiscalYearStart = table.Column<string>(type: "text", nullable: true),
                EnableOnlineGiving = table.Column<bool>(type: "boolean", nullable: false),
                EnableMemberPortal = table.Column<bool>(type: "boolean", nullable: false),
                EnableAttendanceTracking = table.Column<bool>(type: "boolean", nullable: false),
                GoogleWorkspaceDomain = table.Column<string>(type: "text", nullable: true),
                GoogleClientId = table.Column<string>(type: "text", nullable: true),
                GoogleClientSecret = table.Column<string>(type: "text", nullable: true),
                GoogleWorkspaceEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrganizationSettings", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrganizationSettings_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserOrganizationRoles",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false),
                Role = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserOrganizationRoles", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserOrganizationRoles_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AttendanceRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AttendanceSessionId = table.Column<int>(type: "integer", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                IsPresent = table.Column<bool>(type: "boolean", nullable: false),
                IsFirstTime = table.Column<bool>(type: "boolean", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                    column: x => x.AttendanceSessionId,
                    principalTable: "AttendanceSessions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AttendanceRecords_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CommunicationLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Type = table.Column<int>(type: "integer", nullable: false),
                Subject = table.Column<string>(type: "text", nullable: false),
                Body = table.Column<string>(type: "text", nullable: false),
                Recipient = table.Column<string>(type: "text", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ErrorMessage = table.Column<string>(type: "text", nullable: true),
                TemplateId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommunicationLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_CommunicationLogs_CommunicationTemplates_TemplateId",
                    column: x => x.TemplateId,
                    principalTable: "CommunicationTemplates",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_CommunicationLogs_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "DonationPledges",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                DonationFundId = table.Column<int>(type: "integer", nullable: false),
                TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                AmountPerPeriod = table.Column<decimal>(type: "numeric", nullable: false),
                Frequency = table.Column<int>(type: "integer", nullable: false),
                StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Notes = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DonationPledges", x => x.Id);
                table.ForeignKey(
                    name: "FK_DonationPledges_DonationFunds_DonationFundId",
                    column: x => x.DonationFundId,
                    principalTable: "DonationFunds",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_DonationPledges_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Events",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                Visibility = table.Column<int>(type: "integer", nullable: false),
                StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsAllDay = table.Column<bool>(type: "boolean", nullable: false),
                IsRecurring = table.Column<bool>(type: "boolean", nullable: false),
                RecurrenceRule = table.Column<string>(type: "text", nullable: true),
                Location = table.Column<string>(type: "text", nullable: true),
                OnlineUrl = table.Column<string>(type: "text", nullable: true),
                MaxCapacity = table.Column<int>(type: "integer", nullable: true),
                RequiresRegistration = table.Column<bool>(type: "boolean", nullable: false),
                RegistrationDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ImageUrl = table.Column<string>(type: "text", nullable: true),
                CoordinatorMemberId = table.Column<int>(type: "integer", nullable: true),
                CoordinatorId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Events", x => x.Id);
                table.ForeignKey(
                    name: "FK_Events_Members_CoordinatorId",
                    column: x => x.CoordinatorId,
                    principalTable: "Members",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Groups",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                GroupTypeId = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LeaderMemberId = table.Column<int>(type: "integer", nullable: true),
                LeaderId = table.Column<int>(type: "integer", nullable: true),
                CoLeaderMemberId = table.Column<int>(type: "integer", nullable: true),
                CoLeaderId = table.Column<int>(type: "integer", nullable: true),
                MeetingFrequency = table.Column<int>(type: "integer", nullable: true),
                MeetingDay = table.Column<string>(type: "text", nullable: true),
                MeetingTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                MeetingLocation = table.Column<string>(type: "text", nullable: true),
                MaxCapacity = table.Column<int>(type: "integer", nullable: true),
                IsOpenToNewMembers = table.Column<bool>(type: "boolean", nullable: false),
                ImageUrl = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Groups", x => x.Id);
                table.ForeignKey(
                    name: "FK_Groups_GroupTypes_GroupTypeId",
                    column: x => x.GroupTypeId,
                    principalTable: "GroupTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Groups_Members_CoLeaderId",
                    column: x => x.CoLeaderId,
                    principalTable: "Members",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Groups_Members_LeaderId",
                    column: x => x.LeaderId,
                    principalTable: "Members",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "MemberTags",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                Tag = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MemberTags", x => x.Id);
                table.ForeignKey(
                    name: "FK_MemberTags_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Sermons",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Title = table.Column<string>(type: "text", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                SermonDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                SpeakerMemberId = table.Column<int>(type: "integer", nullable: true),
                SpeakerId = table.Column<int>(type: "integer", nullable: true),
                SpeakerName = table.Column<string>(type: "text", nullable: true),
                ScriptureReference = table.Column<string>(type: "text", nullable: true),
                SeriesId = table.Column<int>(type: "integer", nullable: true),
                VideoProvider = table.Column<int>(type: "integer", nullable: true),
                VideoUrl = table.Column<string>(type: "text", nullable: true),
                VideoEmbedCode = table.Column<string>(type: "text", nullable: true),
                AudioUrl = table.Column<string>(type: "text", nullable: true),
                NotesUrl = table.Column<string>(type: "text", nullable: true),
                ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                ViewCount = table.Column<int>(type: "integer", nullable: false),
                IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sermons", x => x.Id);
                table.ForeignKey(
                    name: "FK_Sermons_Members_SpeakerId",
                    column: x => x.SpeakerId,
                    principalTable: "Members",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Sermons_SermonSeries_SeriesId",
                    column: x => x.SeriesId,
                    principalTable: "SermonSeries",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "VolunteerAssignments",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                OpportunityId = table.Column<int>(type: "integer", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VolunteerAssignments", x => x.Id);
                table.ForeignKey(
                    name: "FK_VolunteerAssignments_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_VolunteerAssignments_VolunteerOpportunities_OpportunityId",
                    column: x => x.OpportunityId,
                    principalTable: "VolunteerOpportunities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Donations",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                MemberId = table.Column<int>(type: "integer", nullable: true),
                DonationFundId = table.Column<int>(type: "integer", nullable: false),
                Amount = table.Column<decimal>(type: "numeric", nullable: false),
                DonationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                CheckNumber = table.Column<string>(type: "text", nullable: true),
                TransactionId = table.Column<string>(type: "text", nullable: true),
                Notes = table.Column<string>(type: "text", nullable: true),
                IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                PledgeId = table.Column<int>(type: "integer", nullable: true),
                BatchId = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Donations", x => x.Id);
                table.ForeignKey(
                    name: "FK_Donations_DonationBatches_BatchId",
                    column: x => x.BatchId,
                    principalTable: "DonationBatches",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Donations_DonationFunds_DonationFundId",
                    column: x => x.DonationFundId,
                    principalTable: "DonationFunds",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Donations_DonationPledges_PledgeId",
                    column: x => x.PledgeId,
                    principalTable: "DonationPledges",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Donations_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "EventRegistrations",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                EventId = table.Column<int>(type: "integer", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: true),
                GuestName = table.Column<string>(type: "text", nullable: true),
                GuestEmail = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Notes = table.Column<string>(type: "text", nullable: true),
                GuestCount = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EventRegistrations", x => x.Id);
                table.ForeignKey(
                    name: "FK_EventRegistrations_Events_EventId",
                    column: x => x.EventId,
                    principalTable: "Events",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_EventRegistrations_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "GroupMeetings",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GroupId = table.Column<int>(type: "integer", nullable: false),
                MeetingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Topic = table.Column<string>(type: "text", nullable: true),
                Notes = table.Column<string>(type: "text", nullable: true),
                Location = table.Column<string>(type: "text", nullable: true),
                AttendanceCount = table.Column<int>(type: "integer", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroupMeetings", x => x.Id);
                table.ForeignKey(
                    name: "FK_GroupMeetings_Groups_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Groups",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GroupMembers",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                GroupId = table.Column<int>(type: "integer", nullable: false),
                MemberId = table.Column<int>(type: "integer", nullable: false),
                Role = table.Column<int>(type: "integer", nullable: false),
                JoinedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                LeftDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroupMembers", x => x.Id);
                table.ForeignKey(
                    name: "FK_GroupMembers_Groups_GroupId",
                    column: x => x.GroupId,
                    principalTable: "Groups",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GroupMembers_Members_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Members",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SermonTags",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                SermonId = table.Column<int>(type: "integer", nullable: false),
                Tag = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SermonTags", x => x.Id);
                table.ForeignKey(
                    name: "FK_SermonTags_Sermons_SermonId",
                    column: x => x.SermonId,
                    principalTable: "Sermons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AttendanceRecords_AttendanceSessionId",
            table: "AttendanceRecords",
            column: "AttendanceSessionId");

        migrationBuilder.CreateIndex(
            name: "IX_AttendanceRecords_MemberId",
            table: "AttendanceRecords",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_CommunicationLogs_MemberId",
            table: "CommunicationLogs",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_CommunicationLogs_TemplateId",
            table: "CommunicationLogs",
            column: "TemplateId");

        migrationBuilder.CreateIndex(
            name: "IX_DonationPledges_DonationFundId",
            table: "DonationPledges",
            column: "DonationFundId");

        migrationBuilder.CreateIndex(
            name: "IX_DonationPledges_MemberId",
            table: "DonationPledges",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Donations_BatchId",
            table: "Donations",
            column: "BatchId");

        migrationBuilder.CreateIndex(
            name: "IX_Donations_DonationFundId",
            table: "Donations",
            column: "DonationFundId");

        migrationBuilder.CreateIndex(
            name: "IX_Donations_MemberId",
            table: "Donations",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Donations_PledgeId",
            table: "Donations",
            column: "PledgeId");

        migrationBuilder.CreateIndex(
            name: "IX_EventRegistrations_EventId",
            table: "EventRegistrations",
            column: "EventId");

        migrationBuilder.CreateIndex(
            name: "IX_EventRegistrations_MemberId",
            table: "EventRegistrations",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Events_CoordinatorId",
            table: "Events",
            column: "CoordinatorId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMeetings_GroupId",
            table: "GroupMeetings",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMembers_GroupId",
            table: "GroupMembers",
            column: "GroupId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupMembers_MemberId",
            table: "GroupMembers",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_CoLeaderId",
            table: "Groups",
            column: "CoLeaderId");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_GroupTypeId",
            table: "Groups",
            column: "GroupTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_Groups_LeaderId",
            table: "Groups",
            column: "LeaderId");

        migrationBuilder.CreateIndex(
            name: "IX_Members_FamilyId",
            table: "Members",
            column: "FamilyId");

        migrationBuilder.CreateIndex(
            name: "IX_MemberTags_MemberId",
            table: "MemberTags",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_Organizations_ParentOrganizationId",
            table: "Organizations",
            column: "ParentOrganizationId");

        migrationBuilder.CreateIndex(
            name: "IX_OrganizationSettings_OrganizationId",
            table: "OrganizationSettings",
            column: "OrganizationId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Sermons_SeriesId",
            table: "Sermons",
            column: "SeriesId");

        migrationBuilder.CreateIndex(
            name: "IX_Sermons_SpeakerId",
            table: "Sermons",
            column: "SpeakerId");

        migrationBuilder.CreateIndex(
            name: "IX_SermonTags_SermonId",
            table: "SermonTags",
            column: "SermonId");

        migrationBuilder.CreateIndex(
            name: "IX_UserOrganizationRoles_OrganizationId",
            table: "UserOrganizationRoles",
            column: "OrganizationId");

        migrationBuilder.CreateIndex(
            name: "IX_VolunteerAssignments_MemberId",
            table: "VolunteerAssignments",
            column: "MemberId");

        migrationBuilder.CreateIndex(
            name: "IX_VolunteerAssignments_OpportunityId",
            table: "VolunteerAssignments",
            column: "OpportunityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "AttendanceRecords");

        migrationBuilder.DropTable(
            name: "CommunicationLogs");

        migrationBuilder.DropTable(
            name: "Donations");

        migrationBuilder.DropTable(
            name: "EventRegistrations");

        migrationBuilder.DropTable(
            name: "GroupMeetings");

        migrationBuilder.DropTable(
            name: "GroupMembers");

        migrationBuilder.DropTable(
            name: "MemberTags");

        migrationBuilder.DropTable(
            name: "OrganizationSettings");

        migrationBuilder.DropTable(
            name: "SermonTags");

        migrationBuilder.DropTable(
            name: "UserOrganizationRoles");

        migrationBuilder.DropTable(
            name: "VolunteerAssignments");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "AspNetUsers");

        migrationBuilder.DropTable(
            name: "AttendanceSessions");

        migrationBuilder.DropTable(
            name: "CommunicationTemplates");

        migrationBuilder.DropTable(
            name: "DonationBatches");

        migrationBuilder.DropTable(
            name: "DonationPledges");

        migrationBuilder.DropTable(
            name: "Events");

        migrationBuilder.DropTable(
            name: "Groups");

        migrationBuilder.DropTable(
            name: "Sermons");

        migrationBuilder.DropTable(
            name: "Organizations");

        migrationBuilder.DropTable(
            name: "VolunteerOpportunities");

        migrationBuilder.DropTable(
            name: "DonationFunds");

        migrationBuilder.DropTable(
            name: "GroupTypes");

        migrationBuilder.DropTable(
            name: "Members");

        migrationBuilder.DropTable(
            name: "SermonSeries");

        migrationBuilder.DropTable(
            name: "MemberFamilies");
    }
}
