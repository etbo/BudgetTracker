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
                name: "cc_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: true)
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
                    comment = table.Column<string>(type: "TEXT", nullable: true)
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
                    temps_de_traitement_ms = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cc_import_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "life_insurance_accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    owner = table.Column<string>(type: "TEXT", nullable: false),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_life_insurance_accounts", x => x.id);
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
                    titulaire = table.Column<string>(type: "TEXT", nullable: true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    code = table.Column<string>(type: "TEXT", nullable: false),
                    quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    montant_brut_unitaire = table.Column<double>(type: "REAL", nullable: false),
                    montant_net = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pea_operations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "saving_accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    owner = table.Column<string>(type: "TEXT", nullable: false),
                    bank_name = table.Column<string>(type: "TEXT", nullable: true),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false),
                    update_frequency_in_months = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saving_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cc_operations",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    montant = table.Column<double>(type: "REAL", nullable: false),
                    categorie = table.Column<string>(type: "TEXT", nullable: true),
                    comment = table.Column<string>(type: "TEXT", nullable: true),
                    banque = table.Column<string>(type: "TEXT", nullable: true),
                    date_import = table.Column<DateTime>(type: "TEXT", nullable: false),
                    hash = table.Column<string>(type: "TEXT", nullable: true),
                    import_log_id = table.Column<int>(type: "INTEGER", nullable: true)
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
                name: "life_insurance_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    life_insurance_account_id = table.Column<int>(type: "INTEGER", nullable: false),
                    label = table.Column<string>(type: "TEXT", nullable: false),
                    is_scpi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_life_insurance_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_life_insurance_lines_life_insurance_accounts_life_insurance_account_id",
                        column: x => x.life_insurance_account_id,
                        principalTable: "life_insurance_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saving_statements",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    saving_account_id = table.Column<int>(type: "INTEGER", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saving_statements", x => x.id);
                    table.ForeignKey(
                        name: "FK_saving_statements_saving_accounts_saving_account_id",
                        column: x => x.saving_account_id,
                        principalTable: "saving_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                columns: new[] { "id", "name", "type" },
                values: new object[,]
                {
                    { 1, "Prêt", "Obligatoire" },
                    { 2, "Courses", "Obligatoire" },
                    { 3, "Travaux", "Obligatoire" },
                    { 4, "Loisir", "Loisir" },
                    { 5, "Vacances", "Loisir" },
                    { 6, "Transport", "Obligatoire" },
                    { 7, "Factures", "Obligatoire" },
                    { 8, "Vêtements", "Obligatoire" },
                    { 9, "Cadeaux", "Loisir" },
                    { 10, "Santé", "Obligatoire" },
                    { 11, "Autres", "Obligatoire" },
                    { 12, "Maison/Equip.", "Obligatoire" },
                    { 13, "Neutre", "Neutre" },
                    { 14, "Revenu", "Revenu" }
                });

            migrationBuilder.InsertData(
                table: "cc_category_rules",
                columns: new[] { "id", "category", "comment", "is_used", "max_amount", "max_date", "min_amount", "min_date", "pattern" },
                values: new object[,]
                {
                    { 1, "Santé", "", true, null, null, null, null, "VIR GENERATION" },
                    { 2, "Courses", "", true, null, null, null, null, "MAISON LEGUI" },
                    { 3, "Travaux", "", true, null, null, null, null, "Castorama" },
                    { 4, "Factures", "", true, null, null, null, null, "Sfr" },
                    { 5, "Ammeublement", "", true, null, null, null, null, "edm" },
                    { 6, "Prêt", "", true, null, null, null, null, "Assu. Cnp Pret Habitat" },
                    { 7, "Santé", "", true, null, null, null, null, "Bio's Hair" },
                    { 8, "Courses", "", true, null, null, null, null, "Auchan Le Mans" },
                    { 9, "Courses", "", true, null, null, null, null, "Boulangerie" },
                    { 10, "Courses", "", true, null, null, null, null, "Carrefour City Le Mans" },
                    { 11, "Factures", "", true, null, null, null, null, "PRLV DIRECTION GENERALE DES FINA" },
                    { 12, "Courses", "", true, null, null, null, null, "Carrefour Lemans" },
                    { 13, "Courses", "", true, null, null, null, null, "Casino Shop" },
                    { 14, "Courses", "", true, null, null, null, null, "Eleveurs Regionaux" },
                    { 15, "Courses", "", true, null, null, null, null, "Fournil D'edison" },
                    { 16, "Courses", "", true, null, null, null, null, "Grand*Frais" },
                    { 17, "Courses", "", true, null, null, null, null, "Leclerc*Web" },
                    { 18, "Courses", "", true, null, null, null, null, "Lidl" },
                    { 19, "Courses", "", true, null, null, null, null, "Mgp*la Ruche" },
                    { 20, "Courses", "", true, null, null, null, null, "Mie*Caline" },
                    { 21, "Courses", "", true, null, null, null, null, "U Express" },
                    { 22, "Factures", "", true, null, null, null, null, "Direct*Energie" },
                    { 23, "Factures", "", true, null, null, null, null, "ENI Gas" },
                    { 24, "Factures", "", true, null, null, null, null, "Tres.  Le Mans Ville" },
                    { 25, "Transport", "", true, null, null, null, null, "Auchan Carburant" },
                    { 26, "Transport", "", true, null, null, null, null, "Dac Intermarche" },
                    { 27, "Transport", "", true, null, null, null, null, "E.Leclerc Laval Cedex" },
                    { 28, "Transport", "", true, null, null, null, null, "Relais*Le*Mans*R" },
                    { 29, "Transport", "", true, null, null, null, null, "Stat Leclerc 24/24" },
                    { 30, "Transport", "", true, null, null, null, null, "Station U 72" },
                    { 31, "Factures", "", true, null, null, null, null, "Cabriole" },
                    { 32, "Vêtements", "", true, null, null, null, null, "Esprit Ruaudin" },
                    { 33, "Factures", "", true, null, null, null, null, "Impot" },
                    { 34, "Loisir", "", true, null, null, null, null, "Cultura" },
                    { 35, "Santé", "", true, null, null, null, null, "Harmonie" },
                    { 36, "Santé", "", true, null, null, null, null, "Siaci Saint Honore" },
                    { 37, "Neutre", "", true, null, null, null, null, "Bsa Finances*Virement*Faveur*Malinowski" },
                    { 38, "Transport", "", true, null, null, null, null, "Cofir*Rueil" },
                    { 39, "Prêt", "", true, null, null, null, null, "Realisation De Pret" },
                    { 40, "Prêt", "", true, null, null, null, null, "Remboursement De Pr*t" },
                    { 41, "Loisir", "", true, null, null, null, null, "Bk Rest" },
                    { 42, "Loisir", "", true, null, null, null, null, "Burger King" },
                    { 43, "Loisir", "", true, null, null, null, null, "Crousty" },
                    { 44, "Loisir", "", true, null, null, null, null, "Haochi" },
                    { 45, "Loisir", "", true, null, null, null, null, "Kebab" },
                    { 46, "Loisir", "", true, null, null, null, null, "Lgpo" },
                    { 47, "Loisir", "", true, null, null, null, null, "Mac Donald" },
                    { 48, "Loisir", "", true, null, null, null, null, "Mc Donald" },
                    { 49, "Loisir", "", true, null, null, null, null, "P'tits Ponts" },
                    { 50, "Autre", "", true, null, null, null, null, "Retrait Au Distributeur" },
                    { 51, "Revenu", "", true, null, null, null, null, "Serac" },
                    { 52, "Revenu", "", true, null, null, null, null, "Virement Salaire" },
                    { 53, "Santé", "", true, null, null, null, null, "C.P.A.M." },
                    { 54, "Santé", "", true, null, null, null, null, "Phie" },
                    { 55, "Transport", "", true, null, null, null, null, "Setram" },
                    { 56, "Transport", "", true, null, null, null, null, "Sncf" },
                    { 57, "Travaux", "", true, null, null, null, null, "Brico Depot" },
                    { 58, "Travaux", "", true, null, null, null, null, "Lapeyre" },
                    { 59, "Travaux", "", true, null, null, null, null, "Leroy" },
                    { 60, "Travaux", "", true, null, null, null, null, "Ppg 72" },
                    { 61, "Travaux", "", true, null, null, null, null, "point p" },
                    { 62, "Courses", "", true, null, null, null, null, "LES-BOUCHERS-REGIONAU" },
                    { 63, "Courses", "", true, null, null, null, null, "LES JARDINS VOYAGEURS SAIN" },
                    { 64, "Santé", "", true, null, null, null, null, "DR VIANNAY" },
                    { 65, "Courses", "", true, null, null, null, null, "Biocoop" },
                    { 66, "Santé", "", true, null, null, null, null, "Latrouite" },
                    { 67, "Loisir", "", true, null, null, null, null, "LA CUISINE DU SO LA FERTE" },
                    { 68, "Courses", "", true, null, null, null, null, "LE FENOUIL LE MANS" },
                    { 69, "Santé", "", true, null, null, null, null, "PHARMACIE" },
                    { 70, "Courses", "", true, null, null, null, null, "CLAP BOULANGERI" },
                    { 71, "Courses", "", true, null, null, null, null, "HENRIOT THOMAS" },
                    { 72, "Courses", "", true, null, null, null, null, "MAISON LEGUI LE MANS" },
                    { 73, "Transport", "Essence", true, null, null, null, null, "E.LECLERC LE MANS CEDEX" },
                    { 74, "Assurance", "", true, null, null, null, null, "GAN ASSURANCES PLV CLT" },
                    { 75, "Loisir", "", true, null, null, null, null, "Deezer" },
                    { 76, "Santé", "", true, null, null, null, null, "INGUERE ALICE LE MANS" },
                    { 77, "Factures", "", true, null, null, null, null, "FREE MOBILE" },
                    { 78, "Loisir", "", true, null, null, null, null, "UDEMY" },
                    { 79, "Factures", "", true, null, null, null, null, "PRLV CEDETEL" },
                    { 80, "Courses", "", true, null, null, null, null, "PALAIS DES THES" },
                    { 81, "Factures", "", true, null, null, null, null, "TotalEnergies" },
                    { 82, "Santé", "", true, null, null, null, null, "Psychomot" },
                    { 83, "Courses", "", true, null, null, null, null, "BELLE GARDE SAVIGNE" },
                    { 84, "Revenu", "", true, null, null, null, null, "kdopapymamie" },
                    { 85, "Loisir", "", true, null, null, null, null, "PN LOISIRS PARIS" },
                    { 86, "Loisir", "Manège", true, null, null, null, null, "CREATTITUDE" },
                    { 87, "Transport", "Essence", true, null, null, null, null, "RELAIS MANS RHIN LE MANS" },
                    { 88, "Loisir", "Mondial Tissus", true, null, null, null, null, "GMT M374" },
                    { 89, "Santé", "", true, null, null, null, null, "MAMIE MESURE LE MANS" },
                    { 90, "Loisir", "", true, null, null, null, null, "Google Payment I Dublin" },
                    { 91, "Revenu", "", true, null, null, null, null, "VIR CAF DE LA SARTHE" },
                    { 92, "Prêt", "", true, null, null, null, null, "Appro remboursement Pret" },
                    { 93, "Factures", "Impôts", true, null, null, null, null, "PRLV DIRECTION GENERALE DES FINA 2" },
                    { 94, "Courses", "Café BOC", true, null, null, null, null, "DECA DEVELOPPEME ST MAIXENT" },
                    { 95, "Santé", "", true, null, null, null, null, "PHARMA UNIVERSIT LE MANS" },
                    { 96, "Courses", "", true, null, null, null, null, "JARDINS DE BELLE 72460" },
                    { 97, "Santé", "", true, null, null, null, null, "PHARMA BEAUREGAR" },
                    { 98, "Transport", "", true, null, null, null, null, "ST0072 LE MANS" },
                    { 99, "Loisir", "", true, null, null, null, null, "S EVEILLER SIMPL 69290 CRAPONNE" },
                    { 100, "Factures", "", true, null, null, null, null, "PRLV ECOLE SAINT PAVIN" },
                    { 101, "Santé", "", true, null, null, null, null, "L'ARTISANE LE MANS" },
                    { 102, "Loisir", "", true, null, null, null, null, "TIDAL Malmo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cc_operations_import_log_id",
                table: "cc_operations",
                column: "import_log_id");

            migrationBuilder.CreateIndex(
                name: "IX_life_insurance_lines_life_insurance_account_id",
                table: "life_insurance_lines",
                column: "life_insurance_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_life_insurance_statements_life_insurance_line_id",
                table: "life_insurance_statements",
                column: "life_insurance_line_id");

            migrationBuilder.CreateIndex(
                name: "IX_saving_statements_saving_account_id",
                table: "saving_statements",
                column: "saving_account_id");
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
                name: "saving_accounts");

            migrationBuilder.DropTable(
                name: "life_insurance_accounts");
        }
    }
}
