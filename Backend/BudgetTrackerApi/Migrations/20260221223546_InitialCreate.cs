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
                    { 2, "Courses", "", true, null, null, null, null, null, "MAISON LEGUI", 0 },
                    { 3, "Travaux", "", true, null, null, null, null, null, "Castorama", 0 },
                    { 4, "Factures", "", true, null, null, null, null, null, "Sfr", 0 },
                    { 5, "Ammeublement", "", true, null, null, null, null, null, "edm", 0 },
                    { 6, "Prêt", "", true, null, null, null, null, null, "Assu. Cnp Pret Habitat", 0 },
                    { 7, "Santé", "", true, null, null, null, null, null, "Bio's Hair", 0 },
                    { 8, "Courses", "", true, null, null, null, null, null, "Auchan Le Mans", 0 },
                    { 9, "Courses", "", true, null, null, null, null, null, "Boulangerie", 0 },
                    { 10, "Courses", "", true, null, null, null, null, null, "Carrefour City Le Mans", 0 },
                    { 11, "Factures", "", true, null, null, null, null, null, "PRLV DIRECTION GENERALE DES FINA", 0 },
                    { 12, "Courses", "", true, null, null, null, null, null, "Carrefour Lemans", 0 },
                    { 13, "Courses", "", true, null, null, null, null, null, "Casino Shop", 0 },
                    { 14, "Courses", "", true, null, null, null, null, null, "Eleveurs Regionaux", 0 },
                    { 15, "Courses", "", true, null, null, null, null, null, "Fournil D'edison", 0 },
                    { 16, "Courses", "", true, null, null, null, null, null, "Grand*Frais", 0 },
                    { 17, "Courses", "", true, null, null, null, null, null, "Leclerc*Web", 0 },
                    { 18, "Courses", "", true, null, null, null, null, null, "Lidl", 0 },
                    { 19, "Courses", "", true, null, null, null, null, null, "Mgp*la Ruche", 0 },
                    { 20, "Courses", "", true, null, null, null, null, null, "Mie*Caline", 0 },
                    { 21, "Courses", "", true, null, null, null, null, null, "U Express", 0 },
                    { 22, "Factures", "", true, null, null, null, null, null, "Direct*Energie", 0 },
                    { 23, "Factures", "", true, null, null, null, null, null, "ENI Gas", 0 },
                    { 24, "Factures", "", true, null, null, null, null, null, "Tres.  Le Mans Ville", 0 },
                    { 25, "Transport", "", true, null, null, null, null, null, "Auchan Carburant", 0 },
                    { 26, "Transport", "", true, null, null, null, null, null, "Dac Intermarche", 0 },
                    { 27, "Transport", "", true, null, null, null, null, null, "E.Leclerc Laval Cedex", 0 },
                    { 28, "Transport", "", true, null, null, null, null, null, "Relais*Le*Mans*R", 0 },
                    { 29, "Transport", "", true, null, null, null, null, null, "Stat Leclerc 24/24", 0 },
                    { 30, "Transport", "", true, null, null, null, null, null, "Station U 72", 0 },
                    { 31, "Factures", "", true, null, null, null, null, null, "Cabriole", 0 },
                    { 32, "Vêtements", "", true, null, null, null, null, null, "Esprit Ruaudin", 0 },
                    { 33, "Factures", "", true, null, null, null, null, null, "Impot", 0 },
                    { 34, "Loisir", "", true, null, null, null, null, null, "Cultura", 0 },
                    { 35, "Santé", "", true, null, null, null, null, null, "Harmonie", 0 },
                    { 36, "Santé", "", true, null, null, null, null, null, "Siaci Saint Honore", 0 },
                    { 37, "Neutre", "", true, null, null, null, null, null, "Bsa Finances*Virement*Faveur*Malinowski", 0 },
                    { 38, "Transport", "", true, null, null, null, null, null, "Cofir*Rueil", 0 },
                    { 39, "Prêt", "", true, null, null, null, null, null, "Realisation De Pret", 0 },
                    { 40, "Prêt", "", true, null, null, null, null, null, "Remboursement De Pr*t", 0 },
                    { 41, "Loisir", "", true, null, null, null, null, null, "Bk Rest", 0 },
                    { 42, "Loisir", "", true, null, null, null, null, null, "Burger King", 0 },
                    { 43, "Loisir", "", true, null, null, null, null, null, "Crousty", 0 },
                    { 44, "Loisir", "", true, null, null, null, null, null, "Haochi", 0 },
                    { 45, "Loisir", "", true, null, null, null, null, null, "Kebab", 0 },
                    { 46, "Loisir", "", true, null, null, null, null, null, "Lgpo", 0 },
                    { 47, "Loisir", "", true, null, null, null, null, null, "Mac Donald", 0 },
                    { 48, "Loisir", "", true, null, null, null, null, null, "Mc Donald", 0 },
                    { 49, "Loisir", "", true, null, null, null, null, null, "P'tits Ponts", 0 },
                    { 50, "Autre", "", true, null, null, null, null, null, "Retrait Au Distributeur", 0 },
                    { 51, "Revenu", "", true, null, null, null, null, null, "Serac", 0 },
                    { 52, "Revenu", "", true, null, null, null, null, null, "Virement Salaire", 0 },
                    { 53, "Santé", "", true, null, null, null, null, null, "C.P.A.M.", 0 },
                    { 54, "Santé", "", true, null, null, null, null, null, "Phie", 0 },
                    { 55, "Transport", "", true, null, null, null, null, null, "Setram", 0 },
                    { 56, "Transport", "", true, null, null, null, null, null, "Sncf", 0 },
                    { 57, "Travaux", "", true, null, null, null, null, null, "Brico Depot", 0 },
                    { 58, "Travaux", "", true, null, null, null, null, null, "Lapeyre", 0 },
                    { 59, "Travaux", "", true, null, null, null, null, null, "Leroy", 0 },
                    { 60, "Travaux", "", true, null, null, null, null, null, "Ppg 72", 0 },
                    { 61, "Travaux", "", true, null, null, null, null, null, "point p", 0 },
                    { 62, "Courses", "", true, null, null, null, null, null, "LES-BOUCHERS-REGIONAU", 0 },
                    { 63, "Courses", "", true, null, null, null, null, null, "LES JARDINS VOYAGEURS SAIN", 0 },
                    { 64, "Santé", "", true, null, null, null, null, null, "DR VIANNAY", 0 },
                    { 65, "Courses", "", true, null, null, null, null, null, "Biocoop", 0 },
                    { 66, "Santé", "", true, null, null, null, null, null, "Latrouite", 0 },
                    { 67, "Loisir", "", true, null, null, null, null, null, "LA CUISINE DU SO LA FERTE", 0 },
                    { 68, "Courses", "", true, null, null, null, null, null, "LE FENOUIL LE MANS", 0 },
                    { 69, "Santé", "", true, null, null, null, null, null, "PHARMACIE", 0 },
                    { 70, "Courses", "", true, null, null, null, null, null, "CLAP BOULANGERI", 0 },
                    { 71, "Courses", "", true, null, null, null, null, null, "HENRIOT THOMAS", 0 },
                    { 72, "Courses", "", true, null, null, null, null, null, "MAISON LEGUI LE MANS", 0 },
                    { 73, "Transport", "Essence", true, null, null, null, null, null, "E.LECLERC LE MANS CEDEX", 0 },
                    { 74, "Assurance", "", true, null, null, null, null, null, "GAN ASSURANCES PLV CLT", 0 },
                    { 75, "Loisir", "", true, null, null, null, null, null, "Deezer", 0 },
                    { 76, "Santé", "", true, null, null, null, null, null, "INGUERE ALICE LE MANS", 0 },
                    { 77, "Factures", "", true, null, null, null, null, null, "FREE MOBILE", 0 },
                    { 78, "Loisir", "", true, null, null, null, null, null, "UDEMY", 0 },
                    { 79, "Factures", "", true, null, null, null, null, null, "PRLV CEDETEL", 0 },
                    { 80, "Courses", "", true, null, null, null, null, null, "PALAIS DES THES", 0 },
                    { 81, "Factures", "", true, null, null, null, null, null, "TotalEnergies", 0 },
                    { 82, "Santé", "", true, null, null, null, null, null, "Psychomot", 0 },
                    { 83, "Courses", "", true, null, null, null, null, null, "BELLE GARDE SAVIGNE", 0 },
                    { 84, "Revenu", "", true, null, null, null, null, null, "kdopapymamie", 0 },
                    { 85, "Loisir", "", true, null, null, null, null, null, "PN LOISIRS PARIS", 0 },
                    { 86, "Loisir", "Manège", true, null, null, null, null, null, "CREATTITUDE", 0 },
                    { 87, "Transport", "Essence", true, null, null, null, null, null, "RELAIS MANS RHIN LE MANS", 0 },
                    { 88, "Loisir", "Mondial Tissus", true, null, null, null, null, null, "GMT M374", 0 },
                    { 89, "Santé", "", true, null, null, null, null, null, "MAMIE MESURE LE MANS", 0 },
                    { 90, "Loisir", "", true, null, null, null, null, null, "Google Payment I Dublin", 0 },
                    { 91, "Revenu", "", true, null, null, null, null, null, "VIR CAF DE LA SARTHE", 0 },
                    { 92, "Prêt", "", true, null, null, null, null, null, "Appro remboursement Pret", 0 },
                    { 93, "Factures", "Impôts", true, null, null, null, null, null, "PRLV DIRECTION GENERALE DES FINA 2", 0 },
                    { 94, "Courses", "Café BOC", true, null, null, null, null, null, "DECA DEVELOPPEME ST MAIXENT", 0 },
                    { 95, "Santé", "", true, null, null, null, null, null, "PHARMA UNIVERSIT LE MANS", 0 },
                    { 96, "Courses", "", true, null, null, null, null, null, "JARDINS DE BELLE 72460", 0 },
                    { 97, "Santé", "", true, null, null, null, null, null, "PHARMA BEAUREGAR", 0 },
                    { 98, "Transport", "", true, null, null, null, null, null, "ST0072 LE MANS", 0 },
                    { 99, "Loisir", "", true, null, null, null, null, null, "S EVEILLER SIMPL 69290 CRAPONNE", 0 },
                    { 100, "Factures", "", true, null, null, null, null, null, "PRLV ECOLE SAINT PAVIN", 0 },
                    { 101, "Santé", "", true, null, null, null, null, null, "L'ARTISANE LE MANS", 0 },
                    { 102, "Loisir", "", true, null, null, null, null, null, "TIDAL Malmo", 0 }
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
