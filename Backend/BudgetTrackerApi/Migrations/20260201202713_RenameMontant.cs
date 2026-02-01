using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameMontant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "montant_net",
                table: "pea_operations",
                newName: "amount_net");

            migrationBuilder.RenameColumn(
                name: "montant_brut_unitaire",
                table: "pea_operations",
                newName: "amount_brut_unitaire");

            migrationBuilder.RenameColumn(
                name: "montant",
                table: "cc_operations",
                newName: "amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount_net",
                table: "pea_operations",
                newName: "montant_net");

            migrationBuilder.RenameColumn(
                name: "amount_brut_unitaire",
                table: "pea_operations",
                newName: "montant_brut_unitaire");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "cc_operations",
                newName: "montant");
        }
    }
}
