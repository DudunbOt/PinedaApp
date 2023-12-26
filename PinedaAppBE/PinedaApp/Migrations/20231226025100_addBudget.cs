using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinedaApp.Migrations
{
    /// <inheritdoc />
    public partial class addBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Transaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Academic",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Goal = table.Column<double>(type: "float", nullable: false),
                    Current = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_BudgetId",
                table: "Transaction",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_UserId",
                table: "Budget",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Budget_BudgetId",
                table: "Transaction",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Budget_BudgetId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_BudgetId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Transaction");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Academic",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
