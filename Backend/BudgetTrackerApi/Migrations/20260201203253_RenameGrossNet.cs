using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameGrossNet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount_net",
                table: "pea_operations",
                newName: "net_amount");

            migrationBuilder.RenameColumn(
                name: "amount_brut_unitaire",
                table: "pea_operations",
                newName: "gross_unit_amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "net_amount",
                table: "pea_operations",
                newName: "amount_net");

            migrationBuilder.RenameColumn(
                name: "gross_unit_amount",
                table: "pea_operations",
                newName: "amount_brut_unitaire");
        }
    }
}
