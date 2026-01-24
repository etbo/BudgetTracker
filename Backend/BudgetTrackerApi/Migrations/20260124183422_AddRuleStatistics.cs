using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRuleStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_applied_at",
                table: "cc_category_rules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "usage_count",
                table: "cc_category_rules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 5,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 7,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 8,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 9,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 10,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 11,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 12,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 13,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 14,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 15,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 16,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 17,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 18,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 19,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 20,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 21,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 22,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 23,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 24,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 25,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 26,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 27,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 28,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 29,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 30,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 31,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 32,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 33,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 34,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 35,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 36,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 37,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 38,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 39,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 40,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 41,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 42,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 43,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 44,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 45,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 46,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 47,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 48,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 49,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 50,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 51,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 52,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 53,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 54,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 55,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 56,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 57,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 58,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 59,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 60,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 61,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 62,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 63,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 64,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 65,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 66,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 67,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 68,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 69,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 70,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 71,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 72,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 73,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 74,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 75,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 76,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 77,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 78,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 79,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 80,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 81,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 82,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 83,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 84,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 85,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 86,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 87,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 88,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 89,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 90,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 91,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 92,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 93,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 94,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 95,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 96,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 97,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 98,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 99,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 100,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 101,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "cc_category_rules",
                keyColumn: "id",
                keyValue: 102,
                columns: new[] { "last_applied_at", "usage_count" },
                values: new object[] { null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_applied_at",
                table: "cc_category_rules");

            migrationBuilder.DropColumn(
                name: "usage_count",
                table: "cc_category_rules");
        }
    }
}
