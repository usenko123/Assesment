using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CompanyId_Title",
                table: "Vacancies",
                columns: new[] { "CompanyId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name_Address",
                table: "Companies",
                columns: new[] { "Name", "Address" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vacancies_CompanyId_Title",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name_Address",
                table: "Companies");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CompanyId",
                table: "Vacancies",
                column: "CompanyId");
        }
    }
}
