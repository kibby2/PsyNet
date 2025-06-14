using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyNet.Web.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class FixSuperAdminUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Bio", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "472ba632-6133-44a1-b158-6c10bd7d850d", 0, "System Administrator", "7717894b-32a8-4e55-9b32-97771559b802", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "superadmin@psynet.com", false, null, "Admin", false, null, "SUPERADMIN@PSYNET.COM", "SUPERADMIN", "AQAAAAEAACcQAAAAEMa8/m6Ay8/LDOECUHmozo3iQ8D4uEEnL3GOeev6SH7/by8oEBYnOUOO9DFzDnOPeQ==", null, false, null, "51ada72a-ecde-47d9-8eae-3df46910689d", false, "superadmin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d");

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "IdentityUser",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "472ba632-6133-44a1-b158-6c10bd7d850d", 0, "7717894b-32a8-4e55-9b32-97771559b802", "superadmin@psynet.com", false, false, null, "SUPERADMIN@PSYNET.COM", "SUPERADMIN", "AQAAAAEAACcQAAAAEMa8/m6Ay8/LDOECUHmozo3iQ8D4uEEnL3GOeev6SH7/by8oEBYnOUOO9DFzDnOPeQ==", null, false, "51ada72a-ecde-47d9-8eae-3df46910689d", false, "superadmin" });
        }
    }
}
