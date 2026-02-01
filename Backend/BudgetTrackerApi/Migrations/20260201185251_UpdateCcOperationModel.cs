using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCcOperationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_import",
                table: "cc_operations");

            migrationBuilder.DropColumn(
                name: "is_suggested",
                table: "cc_operations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_import",
                table: "cc_operations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_suggested",
                table: "cc_operations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
