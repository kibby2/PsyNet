using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyNet.Web.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class Hardcodedpasswordhash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37cc67e1-41ca-461c-bf34-2b5e62dbae32",
                column: "NormalizedName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3cfd9eee-08cb-4da3-9e6f-c3166b50d3b0",
                column: "NormalizedName",
                value: "SUPERADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0cab2c3-6558-4a1c-be81-dfb39180da3d",
                column: "NormalizedName",
                value: "USER");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "7717894b-32a8-4e55-9b32-97771559b802", "51ada72a-ecde-47d9-8eae-3df46910689d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37cc67e1-41ca-461c-bf34-2b5e62dbae32",
                column: "NormalizedName",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3cfd9eee-08cb-4da3-9e6f-c3166b50d3b0",
                column: "NormalizedName",
                value: "SuperAdmin");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0cab2c3-6558-4a1c-be81-dfb39180da3d",
                column: "NormalizedName",
                value: "User");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "9a379cea-2074-45a0-86af-fbac353e2c8f", "b62b5b1b-5e03-4339-ab6c-ac5aa5e79eea" });
        }
    }
}
