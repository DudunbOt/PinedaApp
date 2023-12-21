using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinedaApp.Migrations
{
    /// <inheritdoc />
    public partial class addExpertiseAndRenameProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectHandled_Experience_ExperienceId",
                table: "ProjectHandled");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectHandled",
                table: "ProjectHandled");

            migrationBuilder.RenameTable(
                name: "ProjectHandled",
                newName: "Project");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectHandled_ExperienceId",
                table: "Project",
                newName: "IX_Project_ExperienceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Expertise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expertise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expertise_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expertise_UserId",
                table: "Expertise",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Experience_ExperienceId",
                table: "Project",
                column: "ExperienceId",
                principalTable: "Experience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Experience_ExperienceId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "Expertise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "ProjectHandled");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ExperienceId",
                table: "ProjectHandled",
                newName: "IX_ProjectHandled_ExperienceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectHandled",
                table: "ProjectHandled",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectHandled_Experience_ExperienceId",
                table: "ProjectHandled",
                column: "ExperienceId",
                principalTable: "Experience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
