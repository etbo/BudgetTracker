using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetTrackerApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingAccounts2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingStatement_SavingAccount_SavingAccountId",
                table: "SavingStatement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingStatement",
                table: "SavingStatement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingAccount",
                table: "SavingAccount");

            migrationBuilder.RenameTable(
                name: "SavingStatement",
                newName: "saving_statements");

            migrationBuilder.RenameTable(
                name: "SavingAccount",
                newName: "saving_accounts");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "saving_statements",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "saving_statements",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "saving_statements",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "saving_statements",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SavingAccountId",
                table: "saving_statements",
                newName: "saving_account_id");

            migrationBuilder.RenameIndex(
                name: "IX_SavingStatement_SavingAccountId",
                table: "saving_statements",
                newName: "IX_saving_statements_saving_account_id");

            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "saving_accounts",
                newName: "owner");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "saving_accounts",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "saving_accounts",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "saving_accounts",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "BankName",
                table: "saving_accounts",
                newName: "bank_name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_saving_statements",
                table: "saving_statements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_saving_accounts",
                table: "saving_accounts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_saving_statements_saving_accounts_saving_account_id",
                table: "saving_statements",
                column: "saving_account_id",
                principalTable: "saving_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_saving_statements_saving_accounts_saving_account_id",
                table: "saving_statements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_saving_statements",
                table: "saving_statements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_saving_accounts",
                table: "saving_accounts");

            migrationBuilder.RenameTable(
                name: "saving_statements",
                newName: "SavingStatement");

            migrationBuilder.RenameTable(
                name: "saving_accounts",
                newName: "SavingAccount");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "SavingStatement",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "SavingStatement",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "SavingStatement",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SavingStatement",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "saving_account_id",
                table: "SavingStatement",
                newName: "SavingAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_saving_statements_saving_account_id",
                table: "SavingStatement",
                newName: "IX_SavingStatement_SavingAccountId");

            migrationBuilder.RenameColumn(
                name: "owner",
                table: "SavingAccount",
                newName: "Owner");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "SavingAccount",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SavingAccount",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "SavingAccount",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "bank_name",
                table: "SavingAccount",
                newName: "BankName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingStatement",
                table: "SavingStatement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingAccount",
                table: "SavingAccount",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingStatement_SavingAccount_SavingAccountId",
                table: "SavingStatement",
                column: "SavingAccountId",
                principalTable: "SavingAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
