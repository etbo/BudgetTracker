using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLifeInsuranceStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_life_insurance_lines_life_insurance_account_id",
                table: "life_insurance_lines",
                column: "life_insurance_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "life_insurance_lines");

            migrationBuilder.DropTable(
                name: "life_insurance_statements");

            migrationBuilder.DropTable(
                name: "life_insurance_accounts");
        }
    }
}
