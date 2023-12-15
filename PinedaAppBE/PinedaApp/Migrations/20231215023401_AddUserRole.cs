using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinedaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageFilePath",
                table: "Portfolio",
                newName: "ImageFilePath");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "Users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ImageFilePath",
                table: "Portfolio",
                newName: "imageFilePath");
        }
    }
}
