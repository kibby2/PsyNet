using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PsyNet.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToUrlHandle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UrlHandle",
                table: "BlogPosts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_UrlHandle",
                table: "BlogPosts",
                column: "UrlHandle",
                unique: true,
                filter: "[UrlHandle] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_UrlHandle",
                table: "BlogPosts");

            migrationBuilder.AlterColumn<string>(
                name: "UrlHandle",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
