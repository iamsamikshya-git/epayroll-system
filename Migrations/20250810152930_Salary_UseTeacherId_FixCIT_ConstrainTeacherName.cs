using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_PayRoll.Migrations
{
    /// <inheritdoc />
    public partial class Salary_UseTeacherId_FixCIT_ConstrainTeacherName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CitizenInvesmentTrust",
                table: "Salaries",
                newName: "TeacherId");

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "TeacherName",
                table: "Salaries",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CitizenInvestmentTrust",
                table: "Salaries",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "CitizenInvestmentTrust",
                table: "Salaries");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "Salaries",
                newName: "CitizenInvesmentTrust");

            migrationBuilder.AlterColumn<string>(
                name: "TeacherName",
                table: "Salaries",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
