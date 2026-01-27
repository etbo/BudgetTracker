using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFrequencyToLifeInsurance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "update_frequency_in_months",
                table: "life_insurance_accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "update_frequency_in_months",
                table: "life_insurance_accounts");
        }
    }
}
