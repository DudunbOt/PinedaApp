using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinedaApp.Migrations
{
    /// <inheritdoc />
    public partial class addExperienceShortDescColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDesc",
                table: "Experience",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDesc",
                table: "Experience");
        }
    }
}
