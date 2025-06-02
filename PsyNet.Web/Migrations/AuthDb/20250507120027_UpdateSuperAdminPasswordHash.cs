using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyNet.Web.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class UpdateSuperAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEMa8/m6Ay8/LDOECUHmozo3iQ8D4uEEnL3GOeev6SH7/by8oEBYnOUOO9DFzDnOPeQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHq0Dd093TQaKmK4EbOfFTuf4RST7yqWdh7FhvwxRUjL+XHcB2q3Al33ZU5C7tJjkA==");
        }
    }
}
