using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSuggestedToOperation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_suggested",
                table: "cc_operations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_suggested",
                table: "cc_operations");
        }
    }
}
