using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PayRoll.Migrations
{
    /// <inheritdoc />
    public partial class Salary_AddTotals_And_CITDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CitizenInvestmentTrust",
                table: "Salaries",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAllowance",
                table: "Salaries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSalary",
                table: "Salaries",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAllowance",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "TotalSalary",
                table: "Salaries");

            migrationBuilder.AlterColumn<string>(
                name: "CitizenInvestmentTrust",
                table: "Salaries",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
