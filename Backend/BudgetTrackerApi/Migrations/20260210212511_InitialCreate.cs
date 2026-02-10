using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    owner = table.Column<string>(type: "TEXT", nullable: false),
                    bank_name = table.Column<string>(type: "TEXT", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    update_frequency_in_months = table.Column<int>(type: "INTEGER", nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cc_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    macro_category = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cc_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cc_category_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    is_used = table.Column<bool>(type: "INTEGER", nullable: false),
                    pattern = table.Column<string>(type: "TEXT", nullable: true),
                    min_amount = table.Column<double>(type: "REAL", nullable: true),
                    max_amount = table.Column<double>(type: "REAL", nullable: true),
                    min_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    max_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    category = table.Column<string>(type: "TEXT", nullable: true),
                    comment = table.Column<string>(type: "TEXT", nullable: true),
                    usage_count = table.Column<int>(type: "INTEGER", nullable: false),
                    last_applied_at = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cc_category_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cc_import_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    file_name = table.Column<string>(type: "TEXT", nullable: false),
                    import_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    is_successful = table.Column<bool>(type: "INTEGER", nullable: false),
                    msg_erreur = table.Column<string>(type: "TEXT", nullable: false),
                    bank_name = table.Column<string>(type: "TEXT", nullable: true),
                    total_rows = table.Column<int>(type: "INTEGER", nullable: false),
                    inserted_rows = table.Column<int>(type: "INTEGER", nullable: false),
                    date_min = table.Column<DateTime>(type: "TEXT", nullable: true),
                    date_max = table.Column<DateTime>(type: "TEXT", nullable: true),
                    processing_time_ms = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cc_import_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pea_cached_stock_prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ticker = table.Column<string>(type: "TEXT", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    price = table.Column<decimal>(type: "TEXT", nullable: false),
                    cache_timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pea_cached_stock_prices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pea_operations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    owner = table.Column<string>(type: "TEXT", nullable: true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    gross_unit_amount = table.Column<double>(type: "REAL", nullable: false),
                    net_amount = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pea_operations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "life_insurance_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    account_id = table.Column<int>(type: "INTEGER", nullable: false),
                    label = table.Column<string>(type: "TEXT", nullable: false),
                    is_scpi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_life_insurance_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_life_insurance_lines_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saving_statements",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    account_id = table.Column<int>(type: "INTEGER", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saving_statements", x => x.id);
                    table.ForeignKey(
                        name: "FK_saving_statements_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cc_operations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    amount = table.Column<double>(type: "REAL", nullable: false),
                    category = table.Column<string>(type: "TEXT", nullable: true),
                    comment = table.Column<string>(type: "TEXT", nullable: true),
                    bank = table.Column<string>(type: "TEXT", nullable: true),
                    import_log_id = table.Column<int>(type: "INTEGER", nullable: true),
                    hash = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cc_operations", x => x.id);
                    table.ForeignKey(
                        name: "FK_cc_operations_cc_import_logs_import_log_id",
                        column: x => x.import_log_id,
                        principalTable: "cc_import_logs",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "life_insurance_statements",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    life_insurance_line_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    unit_count = table.Column<decimal>(type: "TEXT", nullable: false),
                    unit_value = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_life_insurance_statements", x => x.id);
                    table.ForeignKey(
                        name: "FK_life_insurance_statements_life_insurance_lines_life_insurance_line_id",
                        column: x => x.life_insurance_line_id,
                        principalTable: "life_insurance_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "cc_categories",
                columns: new[] { "id", "macro_category", "name" },
                values: new object[,]
                {
                    { 1, "Obligatoire", "Prêt" },
                    { 2, "Obligatoire", "Courses" },
                    { 3, "Obligatoire", "Travaux" },
                    { 4, "Loisir", "Loisir" },
                    { 5, "Loisir", "Vacances" },
                    { 6, "Obligatoire", "Transport" },
                    { 7, "Obligatoire", "Factures" },
                    { 8, "Obligatoire", "Vêtements" },
                    { 9, "Loisir", "Cadeaux" },
                    { 10, "Obligatoire", "Santé" },
                    { 11, "Obligatoire", "Autres" },
                    { 12, "Obligatoire", "Maison/Equip." },
                    { 13, "Neutre", "Neutre" },
                    { 14, "Revenu", "Revenu" }
                });

            migrationBuilder.InsertData(
                table: "cc_category_rules",
                columns: new[] { "id", "category", "comment", "is_used", "last_applied_at", "max_amount", "max_date", "min_amount", "min_date", "pattern", "usage_count" },
                values: new object[,]
                {
                    { 1, "Santé", "", true, null, null, null, null, null, "VIR GENERATION", 0 },
                    { 2, "Courses", "", true, null, null, null, null, null, "MAISON LEGUI", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cc_operations_import_log_id",
                table: "cc_operations",
                column: "import_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_life_insurance_lines_account_id",
                table: "life_insurance_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_life_insurance_statements_life_insurance_line_id",
                table: "life_insurance_statements",
                column: "life_insurance_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_saving_statements_account_id",
                table: "saving_statements",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cc_categories");

            migrationBuilder.DropTable(
                name: "cc_category_rules");

            migrationBuilder.DropTable(
                name: "cc_operations");

            migrationBuilder.DropTable(
                name: "life_insurance_statements");

            migrationBuilder.DropTable(
                name: "pea_cached_stock_prices");

            migrationBuilder.DropTable(
                name: "pea_operations");

            migrationBuilder.DropTable(
                name: "saving_statements");

            migrationBuilder.DropTable(
                name: "cc_import_logs");

            migrationBuilder.DropTable(
                name: "life_insurance_lines");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
